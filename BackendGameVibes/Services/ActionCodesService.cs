using BackendGameVibes.Data;
using BackendGameVibes.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;

namespace BackendGameVibes.Services {
    public class ActionCodesService {
        private readonly ApplicationDbContext _context;
        private readonly int[] ValidChars = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        public ActionCodesService(ApplicationDbContext applicationDbContext) {
            _context = applicationDbContext;
        }


        public async Task<(ActionCode, bool)> GenerateUniqueActionCode(string userId) {
            string? GenerateUniqueActionCode() {
                return RandomNumberGenerator.GetString(
                    choices: "0123456789",
                    length: 6
                );
            }

            var existValidExpiryDate = await _context.ActiveActionCodes.FirstOrDefaultAsync(c => c.UserId == userId);
            if (existValidExpiryDate != null) {
                if (DateTime.Now < existValidExpiryDate.ExpirationDateTime)
                    return (existValidExpiryDate, true);
                else {
                    _context.ActiveActionCodes.Remove(existValidExpiryDate);
                }
            }

            ActionCode? newActionCode;

            newActionCode = new ActionCode() {
                Code = GenerateUniqueActionCode(),
                CreatedDateTime = DateTime.Now,
                ExpirationDateTime = DateTime.Now.AddHours(1),
                UserId = userId
            };

            while (await _context.ActiveActionCodes.AnyAsync(c => c.Code == newActionCode.Code)) {
                newActionCode.Code = GenerateUniqueActionCode();
            }

            _context.ActiveActionCodes.Add(newActionCode);
            await _context.SaveChangesAsync();
            return (newActionCode, false);
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
