using Dapper;
using InternalProcessMgmt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

namespace InternalProcessMgmt.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // خودکار migrate
            await context.Database.MigrateAsync();

            // Seed Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                );
                await context.SaveChangesAsync();
            }

            // Seed admin user
            User adminUser;

            if (!context.Users.Any())
            {
                adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    RoleId = context.Roles.First(r => r.Name == "Admin").RoleId
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
            else
            {
                adminUser = context.Users.First();
            }

            // StoredProcedure برای مایگریشن اتوماتیک
            if (!context.Tasks.Any())
            {
                using var conn = context.Database.GetDbConnection();
                await conn.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Title", "Initial Task");
                parameters.Add("@Description", "First task created by initializer");
                parameters.Add("@CreatedByUserId", adminUser.UserId);
                parameters.Add("@AssignedToUserId", adminUser.UserId);
                parameters.Add("@Status", "Open");
                parameters.Add("@Priority", "High");
                parameters.Add("@DueDate", null);
                parameters.Add("@NewTaskId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await conn.ExecuteAsync("dbo.Task_Insert", parameters, commandType: CommandType.StoredProcedure);

                int newTaskId = parameters.Get<int>("@NewTaskId");

               
                context.TaskHistories.Add(new TaskHistory
                {
                    TaskId = newTaskId,
                    ChangedByUserId = adminUser.UserId,
                    ChangeType = "Created",
                    OldValue = null,
                    NewValue = "Initial Task",
                    ChangedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }
        }
        }
    }