﻿namespace BackendGameVibes.Services;

using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


public class ActionCodesService : IActionCodesService {
    private readonly ApplicationDbContext _context;

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
