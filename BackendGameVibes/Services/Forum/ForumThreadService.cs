using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.IServices.Forum;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Collections.Specialized.BitVector32;


namespace BackendGameVibes.Services.Forum {
    public class ForumThreadService : IForumThreadService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IForumExperienceService _forumExperienceService;
        private readonly IForumPostService _postService;

        public ForumThreadService(ApplicationDbContext context, IMapper mapper, IForumExperienceService forumExperienceService, IForumPostService postService) {
            _context = context;
            _mapper = mapper;
            _forumExperienceService = forumExperienceService;
            _postService = postService;
        }

        public async Task<object> GetThreadsGroupBySectionsAsync(int pageNumber = 1, int threadsInsectionSize = 10) {
            var query = await _context.ForumThreads
                .Include(t => t.Section)
                .GroupBy(t => t.Section!.Name)
                .Select(g => new {
                    SectionName = g.Key,
                    ThreadsCount = g.Count(),
                    Threads = g.OrderByDescending(t => t.CreatedDateTime)
                                .Skip((pageNumber - 1) * threadsInsectionSize)
                                .Take(threadsInsectionSize)
                                .Select(t => new {
                                    t.Id,
                                    t.Title,
                                    t.CreatedDateTime,
                                    t.LastUpdatedDateTime,
                                    userIdOwner = t.UserOwnerId,
                                    usernameOwner = t.UserOwner!.UserName,
                                    LastPostContent = t.Posts!
                                        .OrderByDescending(p => p.CreatedDateTime)
                                        .Select(p => p.Content)
                                        .FirstOrDefault()
                                        ?? "NoLastPost",
                                })
                                .ToArray()
                })
                .ToArrayAsync();

            return new {
                Data = query
            };
        }

        public async Task<object> GetThreadsInSectionAsync(int sectionId, int pageNumber = 1, int threadsInsectionSize = 10) {
            var query = await _context.ForumThreads
                .Where(t => t.SectionId == sectionId)
                .OrderByDescending(t => t.CreatedDateTime)
                .Skip((pageNumber - 1) * threadsInsectionSize)
                .Take(threadsInsectionSize)
                .Include(t => t.Section)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime,
                    userIdOwner = t.UserOwnerId,
                    usernameOwner = t.UserOwner!.UserName,
                    section = t.Section!.Name,
                    LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                })
                .ToArrayAsync();

            int totalThreads = await _context.ForumThreads.Where(t => t.SectionId == sectionId).CountAsync();

            return new {
                TotalThreads = totalThreads,
                PageSize = threadsInsectionSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalThreads / (double)threadsInsectionSize),
                Data = query
            };

        }

        public async Task<object?> GetThreadWithPostsAsync(int threadId, string? userAccessToken = null, int pageNumber = 1, int postsSize = 10) {
            var thread = await _context.ForumThreads
                .Include(t => t.Posts)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime,
                    t.UserOwnerId,
                    usernameOwner = t.UserOwner!.UserName,
                    section = t.Section!.Name
                })
                .FirstOrDefaultAsync(t => t.Id == threadId);

            if (thread == null)
                return null;

            var postsOfThread = await _postService.GetPostsByThreadIdAsync(threadId, userAccessToken, pageNumber, postsSize);

            return new {
                thread,
                postsOfThread
            };
        }

        public async Task<object[]> GetThreadsLandingAsync() {
            return await _context.ForumThreads
                .OrderByDescending(ft => ft.CreatedDateTime)
                .Take(5)
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
                    LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                })
                .ToArrayAsync();
        }

        public async Task<IEnumerable<object>> GetForumSectionsAsync() {
            return await _context.ForumSections
                .Select(s => new {
                    s.Id,
                    s.Name,
                    s.HexColor
                })
                .ToArrayAsync();
        }

        public async Task<IEnumerable<object>> GetForumRolesAsync() {
            var forumRoles = await _context.ForumRoles
             .Select(fr => new {
                 fr.Id,
                 fr.Name,
                 currentThreshold = fr.Threshold,
             })
             .ToArrayAsync();

            return forumRoles;
        }

        public async Task<object> GetThreadsByUserIdAsync(string userId, int pageNumber = 1, int threadsSize = 10) {
            var query = await _context.ForumThreads
                .Where(t => t.UserOwnerId == userId)
                .OrderByDescending(t => t.CreatedDateTime)
                .Skip((pageNumber - 1) * threadsSize)
                .Take(threadsSize)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime,
                    userIdOwner = t.UserOwnerId,
                    usernameOwner = t.UserOwner!.UserName,
                    section = t.Section!.Name,
                    LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                })
                .ToArrayAsync();

            int totalThreads = await _context.ForumThreads.Where(t => t.UserOwnerId == userId).CountAsync();

            return new {
                TotalThreads = totalThreads,
                PageSize = threadsSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalThreads / (double)threadsSize),
                Data = query
            };
        }

        public async Task<object?> GetThreadsByPhraseAsync(string phrase, int pageNumber = 1, int threadsSize = 10) {
            var query = await _context.ForumThreads
                .Where(t => t.Title!.ToLower().Contains(phrase))
                .OrderByDescending(t => t.CreatedDateTime)
                .Skip((pageNumber - 1) * threadsSize)
                .Take(threadsSize)
                .Include(t => t.Section)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.CreatedDateTime,
                    t.LastUpdatedDateTime,
                    userIdOwner = t.UserOwnerId,
                    usernameOwner = t.UserOwner!.UserName,
                    section = t.Section!.Name,
                    LastPostContent = t.Posts!
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Select(p => p.Content)
                        .FirstOrDefault()
                        ?? "NoLastPost",
                })
                .ToArrayAsync();

            int totalThreads = await _context.ForumThreads.Where(t => t.Title!.ToLower().Contains(phrase)).CountAsync();

            return new {
                TotalThreads = totalThreads,
                PageSize = threadsSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalThreads / (double)threadsSize),
                Data = query
            };
        }

        public async Task<ForumThread?> AddThreadAsync(NewForumThreadDTO newForumThreadDTO) {
            ForumThread? newForumThread = _mapper.Map<ForumThread>(newForumThreadDTO);

            var foundSection = await _context.ForumSections.FirstOrDefaultAsync(fs => fs.Id == newForumThreadDTO.SectionId);
            if (foundSection == null)
                return null;

            var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == newForumThreadDTO.UserOwnerId);
            if (foundUser == null)
                return null;

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

        public async Task<IEnumerable<object>> AddSectionAsync(AddSectionDTO addSectionDTO) {
            ForumSection newSection = _mapper.Map<ForumSection>(addSectionDTO);

            _context.ForumSections.Add(newSection);
            await _context.SaveChangesAsync();

            return await GetForumSectionsAsync();
        }

        public async Task<IEnumerable<object>> UpdateSectionAsync(int idSection, AddSectionDTO addSectionDTO) {
            ForumSection? section = await _context.ForumSections.FindAsync(idSection);
            if (section == null)
                return null!;

            section.Name = addSectionDTO.Name;
            section.HexColor = addSectionDTO.HexColor;
            _context.ForumSections.Update(section);

            await _context.SaveChangesAsync();

            return await GetForumSectionsAsync();
        }

        public async Task<IEnumerable<object>> RemoveSectionAsync(int idSection) {
            ForumSection? section = await _context.ForumSections.FirstOrDefaultAsync(s => s.Id == idSection);
            if (section == null)
                return null!;

            _context.ForumSections.Remove(section);
            await _context.SaveChangesAsync();

            return await GetForumSectionsAsync();
        }

        public void Dispose() {
            _context.Dispose();
        }
    }
}
