using BackendGameVibes.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackendGameVibes.Services {
    public class PostService {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context) {
            _context = context;
        }

        //public async Task<ActionResult<IEnumerable<Post>>> GetAllPosts(int idThread) {
        //    //return await _context.Posts.ToListAsync();
        //}
    }
}
