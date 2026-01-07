using InternalProcessMgmt.Models;
using InternalProcessMgmt.Data;
using Microsoft.EntityFrameworkCore;

namespace InternalProcessMgmt.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) { _db = db; }

        public async Task AddAsync(User user) { _db.Users.Add(user); await _db.SaveChangesAsync(); }
        public Task<User?> GetByUsernameAsync(string username) => _db.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Username == username);
        public Task<User?> GetByIdAsync(int id) => _db.Users.FindAsync(id).AsTask();
    }
}
