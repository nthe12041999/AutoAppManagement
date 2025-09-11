using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.Repository.Repositories
{
    /// <summary>
    /// Repository cho quản lý LicenseUser (quan hệ Account + License)
    /// </summary>
    public interface ILicenseUserRepository : IBaseRepository<LicenseUser>
    {
        Task<LicenseUser?> GetActiveLicenseByUserId(long userId);
        Task<List<LicenseUser>> GetLicensesByUserId(long userId);
        Task<List<LicenseUser>> GetUsersByLicenseId(long licenseId);
        Task<List<LicenseUser>> GetExpiringLicenses(int daysAhead = 30);
        Task<bool> IsUserHasActiveLicense(long userId);
        Task<bool> AssignLicenseToUser(long userId, long licenseId, DateTime startDate, DateTime endDate, bool isTrial = false);
        Task<bool> RevokeLicenseFromUser(long userId, long licenseId);
        Task<bool> RenewUserLicense(long userId, long licenseId, DateTime newEndDate);
    }

    /// <summary>
    /// Implementation cho LicenseUserRepository
    /// </summary>
    public class LicenseUserRepository : BaseRepository<LicenseUser>, ILicenseUserRepository
    {
        public LicenseUserRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<LicenseUser?> GetActiveLicenseByUserId(long userId)
        {
            return await _context.Set<LicenseUser>()
                .Include(lu => lu.License)
                .Where(lu => lu.AccountId == userId && lu.IsActive && !lu.IsDeleted)
                .Where(lu => lu.StartDate <= DateTime.UtcNow && lu.EndDate >= DateTime.UtcNow)
                .OrderByDescending(lu => lu.CreatedDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<LicenseUser>> GetLicensesByUserId(long userId)
        {
            return await _context.Set<LicenseUser>()
                .Include(lu => lu.License)
                .Where(lu => lu.AccountId == userId && !lu.IsDeleted)
                .OrderByDescending(lu => lu.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<LicenseUser>> GetUsersByLicenseId(long licenseId)
        {
            return await _context.Set<LicenseUser>()
                .Include(lu => lu.Account)
                .Where(lu => lu.LicenseId == licenseId && !lu.IsDeleted)
                .OrderByDescending(lu => lu.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<LicenseUser>> GetExpiringLicenses(int daysAhead = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);
            
            return await _context.Set<LicenseUser>()
                .Include(lu => lu.License)
                .Include(lu => lu.Account)
                .Where(lu => lu.IsActive && !lu.IsDeleted &&
                           lu.EndDate <= cutoffDate &&
                           lu.EndDate >= DateTime.UtcNow)
                .OrderBy(lu => lu.EndDate)
                .ToListAsync();
        }

        public async Task<bool> IsUserHasActiveLicense(long userId)
        {
            return await _context.Set<LicenseUser>()
                .AnyAsync(lu => lu.AccountId == userId && lu.IsActive && !lu.IsDeleted &&
                              lu.StartDate <= DateTime.UtcNow && lu.EndDate >= DateTime.UtcNow);
        }

        public async Task<bool> AssignLicenseToUser(long userId, long licenseId, DateTime startDate, DateTime endDate, bool isTrial = false)
        {
            try
            {
                var licenseUser = new LicenseUser
                {
                    AccountId = userId,
                    LicenseId = licenseId,
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    IsTrial = isTrial,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _context.Set<LicenseUser>().AddAsync(licenseUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RevokeLicenseFromUser(long userId, long licenseId)
        {
            try
            {
                var licenseUser = await _context.Set<LicenseUser>()
                    .FirstOrDefaultAsync(lu => lu.AccountId == userId && lu.LicenseId == licenseId && !lu.IsDeleted);

                if (licenseUser != null)
                {
                    licenseUser.IsActive = false;
                    licenseUser.UpdatedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RenewUserLicense(long userId, long licenseId, DateTime newEndDate)
        {
            try
            {
                var licenseUser = await _context.Set<LicenseUser>()
                    .FirstOrDefaultAsync(lu => lu.AccountId == userId && lu.LicenseId == licenseId && !lu.IsDeleted);

                if (licenseUser != null)
                {
                    licenseUser.EndDate = newEndDate;
                    licenseUser.UpdatedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}