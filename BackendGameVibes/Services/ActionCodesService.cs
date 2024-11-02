using BackendGameVibes.Data;
using BackendGameVibes.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BackendGameVibes.Services {
    public class ActionCodesService {
        private readonly ApplicationDbContext _context;
        private readonly int[] ValidChars = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        public ActionCodesService(ApplicationDbContext applicationDbContext) {
            _context = applicationDbContext;
        }


        public async Task<ActionCode> GenerateUniqueActionCode(string userId) {
            ActionCode? newActionCode;
            do {
                newActionCode = new ActionCode() {
                    Code = ValidChars.OrderBy(x => EF.Functions.Random()).Take(6).ToString(),
                    CreatedDateTime = DateTime.Now,
                    ExpirationDateTime = DateTime.Now.AddHours(1),
                    UserId = userId
                };

                newActionCode = await _context.ActiveActionCodes.FirstOrDefaultAsync(c => c.Code == newActionCode.Code) != null ? null : newActionCode;
            }
            while (newActionCode == null);

            _context.ActiveActionCodes.Add(newActionCode);

            return newActionCode;
        }

        public async Task<bool> RemoveActionCode(string actionCode) {
            var actionCodeToRemove = await _context.ActiveActionCodes.FirstOrDefaultAsync(c => c.Code == actionCode);
            if (actionCodeToRemove == null)
                return false;

            _context.ActiveActionCodes.Remove(actionCodeToRemove);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
