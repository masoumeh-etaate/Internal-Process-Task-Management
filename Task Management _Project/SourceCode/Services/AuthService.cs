using InternalProcessMgmt.Models;
using InternalProcessMgmt.Data;
using InternalProcessMgmt.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace InternalProcessMgmt.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public AuthService(IUserRepository repo, IConfiguration config, AppDbContext db)
        {
            _repo = repo;
            _config = config;
            _db = db;
        }

        public async Task<User?> RegisterAsync(string username, string email, string password, string roleName)
        {
            
            var exists = await _repo.GetByUsernameAsync(username);
            if (exists != null)
                return null;

           
            var role = await _db.Roles.SingleOrDefaultAsync(r => r.Name == roleName);

            
            if (role == null)
            {
                role = new Role { Name = roleName };
                _db.Roles.Add(role);
                await _db.SaveChangesAsync();
            }

            
            var hashed = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashed,
                RoleId = role.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(user);

            return user;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            
            var user = await _db.Users.Include(u => u.Role)
                                      .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return string.Empty;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return string.Empty;

            var jwt = _config.GetSection("Jwt");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("userId", user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["DurationMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
