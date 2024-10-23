using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.Helpers;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace BackendGameVibes.Services {
    public class ThreadService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ThreadService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ActionResult<ForumThread>> AddThread(ForumThreadDTO thread) {
            ForumPost newForumPost = _mapper.Map<ForumPost>(thread.FirstPost);


            ForumThread? newForumThread = _mapper.Map<ForumThread>(thread);
            newForumThread.Posts!.Add(newForumPost);

            _context.ForumThreads.Add(newForumThread);
            await _context.SaveChangesAsync();

            return newForumThread;
        }

        //public async Task<ActionResult<IEnumerable<Thread>>> GetAllThreads() {

        //}
    }
}
