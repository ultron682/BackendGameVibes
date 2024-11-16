using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Forum;
using BackendGameVibes.Models.DTOs.Reported;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.Helpers;


namespace BackendGameVibes.Services.Forum {
    public class ForumPostService : IForumPostService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IForumExperienceService _forumExperienceService;
        private readonly IJwtTokenService _jwtTokenService;

        public ForumPostService(ApplicationDbContext context,
            IMapper mapper,
            IForumExperienceService forumExperienceService,
            IJwtTokenService jwtTokenService) {
            _context = context;
            _mapper = mapper;
            _forumExperienceService = forumExperienceService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<object> GetPostsByThreadIdAsync(int threadId, string? userAccessToken = null, int pageNumber = 1, int postsSize = 10) {
            string? userId = null;
            if (userAccessToken != null)
                userId = _jwtTokenService.GetTokenClaims(userAccessToken).userId ?? null;

            var query = await _context.ForumPosts
                .Where(p => p.ThreadId == threadId)
                .Include(p => p.PostInteractions)
                .OrderByDescending(t => t.CreatedDateTime)
                .Skip((pageNumber - 1) * postsSize)
                .Take(postsSize)
                .Select(p => new {
                    p.Id,
                    p.Content,
                    p.CreatedDateTime,
                    p.LikesCount,
                    p.DisLikesCount,
                    p.UserOwnerId,
                    username = p.UserOwner!.UserName,
                    usernameForumRole = p.UserOwner!.ForumRole,
                    userPostInteraction = p.PostInteractions!.Where(i => i.UserId == userId)!.FirstOrDefault()
                })
                .ToArrayAsync();

            int totalPosts = await _context.ForumPosts.Where(p => p.ThreadId == threadId).CountAsync();

            return new {
                TotalPosts = totalPosts,
                PageSize = postsSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalPosts / (double)postsSize),
                Data = query
            };
        }

        public async Task<object> GetPostsByUserIdAsync(string userId, int pageNumber = 1, int postsSize = 10) {
            var query = await _context.ForumPosts
            .Where(p => p.UserOwnerId == userId)
            .OrderByDescending(t => t.CreatedDateTime)
            .Skip((pageNumber - 1) * postsSize)
            .Take(postsSize)
            .Select(p => new {
                p.Id,
                p.Content,
                p.ThreadId,
                p.CreatedDateTime,
                p.LikesCount,
                p.DisLikesCount,
                p.UserOwnerId,
                username = p.UserOwner!.UserName,
                usernameForumRole = p.UserOwner!.ForumRole,
                userPostInteraction = p.PostInteractions!.Where(i => i.UserId == userId)!.FirstOrDefault()
            })
            .ToArrayAsync();

            int totalPosts = await _context.ForumPosts.Where(p => p.UserOwnerId == userId).CountAsync();

            return new {
                TotalPosts = totalPosts,
                PageSize = postsSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalPosts / (double)postsSize),
                Data = query
            };
        }

        public async Task<object?> GetPostByIdAsync(int postId) {
            return await _context.ForumPosts
                .Include(p => p.UserOwner)
                .Select(p => new {
                    p.Id,
                    p.Content,
                    p.ThreadId,
                    p.CreatedDateTime,
                    p.LikesCount,
                    p.DisLikesCount,
                    p.UserOwnerId,
                    username = p.UserOwner!.UserName,
                    usernameForumRole = p.UserOwner!.ForumRole,
                })
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<object?> GetPostsByPhraseAsync(string phrase, int pageNumber = 1, int postsSize = 10) {
            var query = await _context.ForumPosts
                .Where(t => t.Content!.ToLower().Contains(phrase))
                .OrderByDescending(t => t.CreatedDateTime)
                .Skip((pageNumber - 1) * postsSize)
                .Take(postsSize)
                .Select(p => new {
                    p.Id,
                    p.Content,
                    p.ThreadId,
                    p.CreatedDateTime,
                    p.LikesCount,
                    p.DisLikesCount,
                    p.UserOwnerId,
                    username = p.UserOwner!.UserName,
                    usernameForumRole = p.UserOwner!.ForumRole,
                })
            .ToArrayAsync();

            int totalPosts = await _context.ForumPosts.Where(t => t.Content!.ToLower().Contains(phrase)).CountAsync();

            return new {
                TotalPosts = totalPosts,
                PageSize = postsSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalPosts / (double)postsSize),
                Data = query
            };
        }

        public async Task<ForumPost?> AddForumPostAsync(ForumPostDTO newForumPostDTO) {
            ForumPost newForumPost = _mapper.Map<ForumPost>(newForumPostDTO);

            var foundThread = await _context.ForumThreads.FirstOrDefaultAsync(ft => ft.Id == newForumPostDTO.ThreadId);
            if (foundThread == null)
                return null;

            var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == newForumPostDTO.UserOwnerId);
            if (foundUser == null)
                return null;

            _context.ForumPosts.Add(newForumPost);
            await _context.SaveChangesAsync();

            await _forumExperienceService.AddPostPoints(newForumPostDTO.UserOwnerId!);

            return newForumPost;
        }

        public async Task<ReportedPost?> ReportPostAsync(string userId, ReportPostDTO reportPostDTO) {
            var forumPost = await _context.ForumPosts.FindAsync(reportPostDTO.ForumPostId);
            if (forumPost == null) {
                return null;
            }
            var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (foundUser == null)
                return null;

            var existingReport = await _context.ReportedForumPosts
                .FirstOrDefaultAsync(rp => rp.ForumPostId == reportPostDTO.ForumPostId && rp.ReporterUserId == userId);

            if (existingReport != null)
                return existingReport;

            ReportedPost newReportPost = _mapper.Map<ReportedPost>(reportPostDTO);
            newReportPost.ReporterUserId = userId;

            _context.ReportedForumPosts.Add(newReportPost);
            await _context.SaveChangesAsync();

            return newReportPost;
        }

        public async Task<ReportedPost?> FinishReportPostAsync(int id, bool toRemovePost) {
            var reportedPost = await _context.ReportedForumPosts.FindAsync(id);
            if (reportedPost == null) {
                return null;
            }

            reportedPost.IsFinished = true;
            _context.ReportedForumPosts.Update(reportedPost);

            if (toRemovePost) {
                var post = await _context.ForumPosts.FindAsync(reportedPost.ForumPostId);
                if (post != null) {
                    _context.ForumPosts.Remove(post);
                }
            }

            await _context.SaveChangesAsync();

            return reportedPost;
        }

        public async Task<ForumPost?> UpdatePostByIdAsync(int postId, string userId, ForumPostUpdateDTO postUpdateDTO) {
            var post = await _context.ForumPosts
               .FirstOrDefaultAsync(p => p.Id == postId && p.UserOwnerId == userId);

            if (post != null) {
                post.Content = postUpdateDTO.Content ?? post.Content;

                post.LastUpdatedDateTime = DateTime.Now;
                _context.ForumPosts.Update(post);
                await _context.SaveChangesAsync();
            }

            return post;
        }

        public async Task<bool> DeletePostByIdAsync(int postId, string userId) {
            var post = await _context.ForumPosts
               .FirstOrDefaultAsync(p => p.Id == postId && p.UserOwnerId == userId);

            if (post != null) {
                _context.ForumPosts.Remove(post);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<object?> InteractPostAsync(string userId, int postId, bool? isToLike) {
            ForumPost? post = await _context.ForumPosts
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) {
                return null;
            }

            ForumPostInteraction? postInteraction = await _context.ForumPostInteractions
                .FirstOrDefaultAsync(pl => pl.UserId == userId && pl.PostId == postId);

            if (postInteraction != null) {
                if (isToLike == null) {
                    if (postInteraction.IsLike == true) {
                        post.LikesCount--;
                    }
                    else if (postInteraction.IsLike == false) {
                        post.DisLikesCount--;
                    }

                    _context.ForumPostInteractions.Remove(postInteraction);
                }
                else if (isToLike == true) {
                    if (postInteraction.IsLike == false) {
                        post.LikesCount++;
                        post.DisLikesCount--;
                    }
                    else if (postInteraction.IsLike == null) {
                        post.LikesCount++;
                    }

                    postInteraction.IsLike = isToLike;
                    _context.ForumPostInteractions.Update(postInteraction);
                }
                else if (isToLike == false) {
                    if (postInteraction.IsLike == true) {
                        post.LikesCount--;
                        post.DisLikesCount++;
                    }
                    else if (postInteraction.IsLike == null) {
                        post.DisLikesCount++;
                    }

                    postInteraction.IsLike = isToLike;
                    _context.ForumPostInteractions.Update(postInteraction);
                }
            }
            else {
                if (isToLike != null) {
                    _context.ForumPostInteractions.Add(new ForumPostInteraction {
                        UserId = userId,
                        PostId = postId,
                        IsLike = isToLike
                    });

                    if (isToLike == true) {
                        post.LikesCount++;
                    }
                    else {
                        post.DisLikesCount++;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return post;
        }


        public void Dispose() {
            _context.Dispose();
        }
    }
}
