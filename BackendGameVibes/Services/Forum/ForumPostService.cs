using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.Requests.Forum;
using BackendGameVibes.Models.Requests.Reported;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

        public async Task<ReportedPost?> ReportPostAsync(string userId, ReportPostDTO reportPostDTO) {
            var forumPost = await _context.ForumPosts.FindAsync(reportPostDTO.ForumPostId);
            if (forumPost == null) {
                return null;
            }

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
        public void Dispose() {
            _context.Dispose();
        }
    }
}
