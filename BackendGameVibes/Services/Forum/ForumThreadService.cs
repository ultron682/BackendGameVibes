using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Services.Forum {
    public class ForumThreadService : IForumThreadService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IForumExperienceService _forumExperienceService;

        public ForumThreadService(ApplicationDbContext context, IMapper mapper, IForumExperienceService forumExperienceService) {
            _context = context;
            _mapper = mapper;
            _forumExperienceService = forumExperienceService;
        }

        public async Task<ForumThread> AddThreadAsync(NewForumThreadDTO newForumThreadDTO) {
            ForumThread? newForumThread = _mapper.Map<ForumThread>(newForumThreadDTO);

            ForumPost newForumPost = new() {
                Content = newForumThreadDTO.FirstForumPostContent,
                ThreadId = newForumThread.Id,
                UserOwnerId = newForumThreadDTO.UserOwnerId
            };
            newForumThread.Posts!.Add(newForumPost);

            _context.ForumThreads.Add(newForumThread);
            await _context.SaveChangesAsync();

            await _forumExperienceService.AddThreadPoints(newForumThreadDTO.UserOwnerId!);

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
                .Include(t => t.Section)
                .Include(t => t.UserOwner)
                .AsSplitQuery()
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime,
                    t.UserOwnerId,
                    usernameOwner = t.UserOwner!.UserName,
                    section = t.Section!.Name,
                })
                .OrderByDescending(ft => ft.CreatedDateTime)
                .Take(5)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetSections() {
            return await _context.ForumSections
                .Select(s => new {
                    s.Id,
                    s.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetForumRoles() {
            return await _context.ForumRoles
             .Select(fr => new {
                 fr.Id,
                 fr.Name
             })
             .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetAllUserThreads(string userId) {
            return await _context.ForumThreads
                .Where(t => t.UserOwnerId == userId)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime
                })
                .OrderByDescending(p => p.CreatedDateTime)
                .ToArrayAsync();
        }

        public void Dispose() {
            _context.Dispose();
        }
    }
}
