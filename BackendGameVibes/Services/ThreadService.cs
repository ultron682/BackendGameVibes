using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.Helpers;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Requests.Forum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace BackendGameVibes.Services {
    public class ThreadService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ThreadService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ForumThread> AddThreadAsync(NewForumThreadDTO newThread) {
            ForumThread? newForumThread = _mapper.Map<ForumThread>(newThread);

            ForumPost newForumPost = new() {
                Content = newThread.FirstForumPostContent,
                ThreadId = newForumThread.Id,
                UserOwnerId = newThread.UserOwnerId
            };
            newForumThread.Posts!.Add(newForumPost);

            _context.ForumThreads.Add(newForumThread);
            await _context.SaveChangesAsync();

            return newForumThread;
        }

        public async Task<IEnumerable<ForumThread>> GetAllThreads() {
            return await _context.ForumThreads
                .Include(t => t.Posts)
                .ToListAsync();
        }
    }
}
