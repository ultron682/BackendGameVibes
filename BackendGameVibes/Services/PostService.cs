using BackendGameVibes.Data;
using BackendGameVibes.Models.Forum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Services {
    public class PostService {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<ForumPost>>> GetAllPosts(int idThread) {
            return await _context.ForumPosts
                .Where(p => p.ThreadId == idThread)
                .ToListAsync();
        }
    }
}
