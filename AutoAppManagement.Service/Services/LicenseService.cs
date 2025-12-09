using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Models.Enums;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutoAppManagement.Service.Services
{
    public interface ILicenseService : IBaseBusinessService<LicenseDTO>
    {
        Task<List<LicenseDTO>> GetLicensesByAccountId(long accountId);
        Task<LicenseDTO> GetLicenseByKey(string licenseKey);
        
        // License assignment methods
        Task<BaseResponse> AssignLicenseToAccount(AssignLicenseToAccountRequest request);
        Task<BaseResponse> AssignLicenseToUser(AssignLicenseToUserRequest request);
        Task<BaseResponse> UnassignLicenseFromAccount(long accountId);
        Task<List<AccountDTO>> GetUsersAssignedToLicense(long licenseId);
        
        // License management methods
        Task<BaseResponse> RenewLicense(RenewLicenseRequest request);
        Task<BaseResponse> SuspendLicense(long id);
        Task<BaseResponse> ActivateLicense(long id);
        Task<List<LicenseDTO>> GetExpiredLicenses();
        Task<List<LicenseDTO>> GetExpiringLicenses(int days);
        Task<BaseResponse> ExtendLicense(long id, DateTime newExpiryDate);
        Task<AutoAppManagement.Models.DTO.License.LicenseStatisticsDTO> GetLicenseStatistics();
    }

    public class LicenseService : BaseBusinessService<License, LicenseDTO, ILicenseRepository>, ILicenseService
    {
        // Lazy load specific repositories 
        private ILicenseRepository? _licenseRepository;
        protected ILicenseRepository LicenseRepository
            => _licenseRepository ??= _serviceProvider.GetRequiredService<ILicenseRepository>();

        private IAccountsRepository? _accountRepository;
        protected IAccountsRepository AccountRepository
            => _accountRepository ??= UnitOfWork.AccountsRepository;

        public LicenseService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Validate và set default values cho License trước khi submit
        /// </summary>
        public override async Task CustomBeforeSubmitData(LicenseDTO dto)
        {
            if (dto.State == Models.Common.EntityState.Add)
            {
                // Set default EndDate nếu null (dựa vào DurationDays)
                if (!dto.EndDate.HasValue && dto.DurationDays > 0)
                {
                    dto.EndDate = dto.StartDate.AddDays(dto.DurationDays);
                }
            }

            await base.CustomBeforeSubmitData(dto);
        }

        public async Task<List<LicenseDTO>> GetLicensesByAccountId(long accountId)
        {
            try
            {
                // Cách 1: Lấy license trực tiếp từ Account.LicenseId
                var account = await AccountRepository.FirstOrDefault(a => a.ID == accountId);
                if (account?.LicenseId != null)
                {
                    var license = await Repository.FirstOrDefault(l => l.ID == account.LicenseId);
                    if (license != null)
                    {
                        return new List<LicenseDTO> { Mapper.Map<LicenseDTO>(license) };
                    }
                }
                return new List<LicenseDTO>();
            }
            catch (Exception)
            {
                return new List<LicenseDTO>();
            }
        }

        public async Task<LicenseDTO> GetLicenseByKey(string licenseKey)
        {
            var license = await Repository.FirstOrDefault(l => l.LicenseKey == licenseKey);
            return Mapper.Map<LicenseDTO>(license);
        }

        public async Task<BaseResponse> AssignLicenseToAccount(AssignLicenseToAccountRequest request)
        {
            try
            {
                // Kiểm tra license tồn tại và hợp lệ
                var license = await Repository.FirstOrDefault(l => l.ID == request.LicenseId);
                if (license == null)
                {
                    return BaseResponse.Error("License không tồn tại");
                }

                if (license.EndDate < DateTime.UtcNow)
                {
                    return BaseResponse.Error("License đã hết hạn");
                }

                // Kiểm tra account tồn tại
                var account = await AccountRepository.FirstOrDefault(a => a.ID == request.AccountId);
                if (account == null)
                {
                    return BaseResponse.Error("Account không tồn tại");
                }

                // Gán license cho account
                account.LicenseId = request.LicenseId;
                account.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success($"Đã gán license '{license.LicenseName}' cho account '{account.Email}' thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gán license cho account: {ex.Message}");
            }
        }

        public async Task<BaseResponse> AssignLicenseToUser(AssignLicenseToUserRequest request)
        {
            try
            {
                // Kiểm tra account tồn tại  
                var account = await AccountRepository.FirstOrDefault(a => a.ID == request.AccountId);
                if (account == null)
                {
                    return BaseResponse.Error("Account không tồn tại");
                }
                account.LicenseId = request.LicenseId;

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success($"Đã gán license cho user '{account.Email}' thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gán license cho user: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UnassignLicenseFromAccount(long accountId)
        {
            try
            {
                var account = await AccountRepository.FirstOrDefault(a => a.ID == accountId);
                if (account == null)
                {
                    return BaseResponse.Error("Account không tồn tại");
                }

                account.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Đã hủy gán license khỏi account thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi hủy gán license: {ex.Message}");
            }
        }

        public async Task<List<AccountDTO>> GetUsersAssignedToLicense(long licenseId)
        {
            try
            {
                var licenseUsers = await AccountRepository.GetByCondition(lu => lu.LicenseId == licenseId);
                return Mapper.Map<List<AccountDTO>>(licenseUsers.ToList());
            }
            catch (Exception)
            {
                return new List<AccountDTO>();
            }
        }

        public async Task<BaseResponse> RenewLicense(AutoAppManagement.Models.DTO.License.RenewLicenseRequest request)
        {
            try
            {
                var license = await UpdateById(request.LicenseId);

                license.EndDate = request.NewExpiryDate;
                license.Status = Models.Enum.StatusEnum.Active;

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<LicenseDTO>(license), "Gia hạn license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gia hạn license: {ex.Message}");
            }
        }

        public async Task<BaseResponse> SuspendLicense(long id)
        {
            try
            {
                var license = await UpdateById(id);

                license.Status = Models.Enum.StatusEnum.Inactive;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Tạm ngưng license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi tạm ngưng license: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ActivateLicense(long id)
        {
            try
            {
                var license = await UpdateById(id);

                license.Status = Models.Enum.StatusEnum.Active;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Kích hoạt license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi kích hoạt license: {ex.Message}");
            }
        }

        public async Task<List<LicenseDTO>> GetExpiredLicenses()
        {
            var licenses = await Repository.GetByCondition(l => l.EndDate < DateTime.UtcNow && l.Status == Models.Enum.StatusEnum.Active);
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        public async Task<List<LicenseDTO>> GetExpiringLicenses(int days)
        {
            var expiryDate = DateTime.UtcNow.AddDays(days);
            var licenses = await Repository.GetByCondition(l => l.EndDate <= expiryDate && l.EndDate > DateTime.UtcNow && l.Status == Models.Enum.StatusEnum.Active);
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        public async Task<BaseResponse> ExtendLicense(long id, DateTime newExpiryDate)
        {
            try
            {
                var license = await UpdateById(id);

                if (newExpiryDate <= license.EndDate)
                {
                    return BaseResponse.Error("Ngày hết hạn mới phải sau ngày hết hạn hiện tại");
                }

                license.EndDate = newExpiryDate;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<LicenseDTO>(license), "Gia hạn license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gia hạn license: {ex.Message}");
            }
        }

        public async Task<AutoAppManagement.Models.DTO.License.LicenseStatisticsDTO> GetLicenseStatistics()
        {
            // Lấy danh sách license và accounts có LicenseId
            var licenseList = (await Repository.GetAll()).ToList();
            var accountsWithLicense = (await AccountRepository.GetByCondition(a => a.LicenseId != 0)).ToList();

            // Nhóm account theo LicenseId -> số KH dùng từng license
            var licenseIdToCustomerCount = accountsWithLicense
                .GroupBy(a => a.LicenseId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Tổng tiền = sum(giá license * số KH dùng license)
            decimal totalRevenue = 0m;
            foreach (var lic in licenseList)
            {
                var count = licenseIdToCustomerCount.TryGetValue(lic.ID, out var c) ? c : 0;
                if (count > 0)
                {
                    totalRevenue += (lic.Price * count);
                }
            }

            var now = DateTime.UtcNow;
            var stats = new AutoAppManagement.Models.DTO.License.LicenseStatisticsDTO
            {
                TotalLicenses = licenseList.Count,
                ActiveLicenses = licenseList.Count(x => x.Status == Models.Enum.StatusEnum.Active),
                SuspendedLicenses = licenseList.Count(x => x.Status == Models.Enum.StatusEnum.Inactive),
                ExpiredLicenses = licenseList.Count(x => x.EndDate != null && x.EndDate < now),
                ExpiringSoonLicenses = licenseList.Count(x => x.EndDate != null && x.EndDate >= now && x.EndDate <= now.AddDays(30)),
                TotalRevenue = totalRevenue,
                MonthlyRevenue = 0
            };

            return stats;
        }

        /// <summary>
        /// Override GetPagingByView để query trực tiếp từ ViewLicense và trả về LicenseDTO (camelCase ở client)
        /// </summary>
        protected override async Task<object?> GetPagingByView(PagingRequestDTO pagingRequestDTO)
        {
            // Chỉ xử lý nếu View là ViewLicense, ngược lại dùng logic mặc định
            if (pagingRequestDTO.View != Models.Enums.EnumView.ViewLicense)
            {
                return null;
            }

            try
            {
                var connection = UnitOfWork.Context.Database.GetDbConnection();

                // Build WHERE + parameters dùng helper chung ở base
                var searchFields = new List<string> { "LicenseName", "LicenseKey", "Description" };
                var (whereClause, parameters) = BuildWhereClauseFromFilters(pagingRequestDTO, searchFields);

                // Tổng số bản ghi
                var countSql = $"SELECT COUNT(*) FROM ViewLicense {whereClause}";
                var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

                // Paging + sort
                var sortField = string.IsNullOrEmpty(pagingRequestDTO.Sort) ? "ID" : pagingRequestDTO.Sort;
                var offset = (pagingRequestDTO.PageIndex - 1) * pagingRequestDTO.PageSize;

                // Cho phép FE chỉ định cột cần SELECT, nhưng vẫn map sang DTO
                var defaultFields = "ID, LicenseKey, LicenseName, LicenseType, Description, MaxDevices, MaxUsers, " +
                                    "StartDate, EndDate, Price, Currency, PaymentInfo, FeatureLimits, Features, " +
                                    "Status, Discount, CreatedBy, UpdatedBy, CreatedDate, UpdatedDate, DurationDays, TotalAccount";
                var selectFields = GetSelectFieldsForView(pagingRequestDTO, defaultFields);

                var dataSql = $@"
                    SELECT {selectFields}
                    FROM ViewLicense
                    {whereClause}
                    ORDER BY [{sortField}]
                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                parameters.Add("@offset", offset);
                parameters.Add("@pageSize", pagingRequestDTO.PageSize);

                // Dapper map về ViewLicenseFullResult rồi manual map sang LicenseDTO
                var viewResults = await connection.QueryAsync<ViewLicenseFullResult>(dataSql, parameters);

                var dtos = viewResults.Select(v => new LicenseDTO
                {
                    ID = v.ID,
                    LicenseKey = v.LicenseKey,
                    LicenseName = v.LicenseName,
                    LicenseType = v.LicenseType,
                    Description = v.Description,
                    MaxDevices = v.MaxDevices ?? 0,
                    MaxUsers = v.MaxUsers ?? 0,
                    StartDate = v.StartDate ?? DateTime.UtcNow,
                    EndDate = v.EndDate,
                    Price = v.Price ?? 0,
                    Currency = v.Currency,
                    PaymentInfo = v.PaymentInfo,
                    FeatureLimits = v.FeatureLimits,
                    Features = v.Features,
                    Status = v.Status.HasValue ? (Models.Enum.StatusEnum)v.Status.Value : Models.Enum.StatusEnum.Active,
                    Discount = v.Discount ?? 0,
                    CreatedBy = v.CreatedBy,
                    UpdatedBy = v.UpdatedBy,
                    CreatedDate = v.CreatedDate,
                    UpdatedDate = v.UpdatedDate,
                    DurationDays = v.DurationDays ?? 0,
                    TotalAccount = v.TotalAccount ?? 0
                }).ToList();

                // Trả về DTO để ASP.NET tự camelCase
                return new PagingResultDTO<LicenseDTO>
                {
                    Data = dtos,
                    TotalItems = totalCount,
                    PageIndex = pagingRequestDTO.PageIndex,
                    PageSize = pagingRequestDTO.PageSize
                };
            }
            catch
            {
                // Lỗi thì fallback về logic GetPaging mặc định
                return null;
            }
        }

        /// <summary>
        /// Class để map toàn bộ kết quả từ ViewLicense view
        /// </summary>
        private class ViewLicenseFullResult
        {
            public long ID { get; set; }
            public string LicenseKey { get; set; } = string.Empty;
            public string LicenseName { get; set; } = string.Empty;
            public string LicenseType { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int? MaxDevices { get; set; }
            public int? MaxUsers { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public decimal? Price { get; set; }
            public string Currency { get; set; } = string.Empty;
            public string? PaymentInfo { get; set; }
            public string? FeatureLimits { get; set; }
            public string? Features { get; set; }
            public int? Status { get; set; }
            public decimal? Discount { get; set; }
            public long? CreatedBy { get; set; }
            public long? UpdatedBy { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public int? DurationDays { get; set; }
            public int? TotalAccount { get; set; }
        }

    }
}
