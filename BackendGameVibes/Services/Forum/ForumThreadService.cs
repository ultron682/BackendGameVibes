using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.Helpers;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Requests.Forum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Threading;

namespace BackendGameVibes.Services.Forum {
    public class ForumThreadService : IForumThreadService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ForumThreadService(ApplicationDbContext context, IMapper mapper) {
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

        public async Task<ForumThread?> GetForumThread(int id) {
            return await _context.ForumThreads
                .Include(t => t.Posts)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<object>> GetLandingThreads() {
            return await _context.ForumThreads
                .Include(t => t.Posts)
                .Include(t => t.Section)
                .Include(t => t.UserOwner)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime,
                    t.UserOwnerId,
                    usernameOwner = t.UserOwner!.UserName,
                    section = t.Section!.Name,
                    //Posts = t.Posts!.Select(p => new {
                    //    p.Id,
                    //    p.Content,
                    //    p.CreatedDateTime,
                    //    p.UserOwnerId,
                    //    p.UserOwner
                    //})
                })
                .OrderByDescending(ft => ft.CreatedDateTime)
                .Take(5)
                .ToListAsync();
        }

        public void Dispose() {
            _context.Dispose();
        }
    }
}
