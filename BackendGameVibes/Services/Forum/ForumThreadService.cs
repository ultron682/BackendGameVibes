using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.IServices.Forum;

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

        public async Task<object?> GetForumThread(int id) {
            return await _context.ForumThreads
                .Include(t => t.Posts)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime,
                    t.UserOwnerId,
                    usernameOwner = t.UserOwner!.UserName,
                    section = t.Section!.Name,
                    LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                })
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<object[]> GetLandingThreads() {
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
                    LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                })
                .OrderByDescending(ft => ft.CreatedDateTime)
                .Take(5)
                .ToArrayAsync();
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
                    t.LastUpdatedDateTime,
                    LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                })
                .OrderByDescending(p => p.CreatedDateTime)
                .ToArrayAsync();
        }

        public async Task<object[]?> GetThreadsByPhrase(string phrase) {
            return await _context.ForumThreads
                .Include(t => t.Section)
                .Where(t => t.Title!.ToLower().Contains(phrase))
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime,
                    section = t.Section!.Name,
                    LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                })
                .OrderByDescending(p => p.CreatedDateTime)
                .ToArrayAsync();
        }


        public async Task<object[]> GetThreadsGroupBySectionsAsync() {
            return await _context.ForumThreads
                .Include(t => t.Section)
                .GroupBy(t => t.Section!.Name)
                .Select(g => new {
                    SectionName = g.Key,
                    ThreadsCount = g.Count(),
                    Threads = g.Select(t => new {
                        t.Id,
                        t.Title,
                        t.CreatedDateTime,
                        t.LastUpdatedDateTime,
                        LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                    })
                    .OrderByDescending(p => p.CreatedDateTime)
                    .ToArray()
                })
                .ToArrayAsync();
        }

        public async Task<object[]> GetThreadsInSectionAsync(int sectionId) {
            return await _context.ForumThreads
                .Include(t => t.Section)
                .Where(t => t.SectionId == sectionId)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime,
                    section = t.Section!.Name,
                    LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                })
                .OrderByDescending(p => p.CreatedDateTime)
                .ToArrayAsync();
        }
    }
}
