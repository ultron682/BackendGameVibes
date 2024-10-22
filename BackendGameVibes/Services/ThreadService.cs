using BackendGameVibes.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackendGameVibes.Services {
    public class ThreadService {
        private readonly ApplicationDbContext _context;

        public ThreadService(ApplicationDbContext context) {
            _context = context;
        }


        //public async Task<ActionResult<IEnumerable<Thread>>> GetAllThreads() {

        //}
    }
}
