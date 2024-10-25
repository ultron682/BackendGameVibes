using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Requests.Forum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Services.Forum {
    public class ForumPostService : IForumPostService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        public ForumPostService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ActionResult<IEnumerable<ForumPost>>> GetAllPosts(int idThread) {
            return await _context.ForumPosts
                .Where(p => p.ThreadId == idThread)
                .ToListAsync();
        }

        public async Task<ForumPost> AddForumPost(ForumPostDTO forumPostDTO) {
            ForumPost newForumPost = _mapper.Map<ForumPost>(forumPostDTO);

            _context.ForumPosts.Add(newForumPost);
            await _context.SaveChangesAsync();

            return newForumPost;
        }

        public void Dispose() {
            _context.Dispose();
        }
    }
}
