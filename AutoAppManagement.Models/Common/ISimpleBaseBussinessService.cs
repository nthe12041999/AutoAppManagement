using AutoAppManagement.Models.Common;

namespace AutoAppManagement.Models.Common
{
    /// <summary>
    /// Interface cơ bản cho Simple Business Service với pattern XXXDTOEdit
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TDto">DTO type</typeparam>
    /// <typeparam name="TDtoEdit">DTO Edit type với EntityState</typeparam>
    public interface ISimpleBaseBussinessService<TEntity, TDto, TDtoEdit>
    {
        /// <summary>
        /// Lấy danh sách có phân trang
        /// </summary>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số lượng item mỗi trang</param>
        /// <param name="searchTerm">Từ khóa tìm kiếm</param>
        /// <returns></returns>
        Task<PagingResponse<TDto>> GetPaging(int page, int pageSize, string? searchTerm = null);

        /// <summary>
        /// Lấy tất cả dữ liệu
        /// </summary>
        /// <returns></returns>
        Task<List<TDto>> GetAll();

        /// <summary>
        /// Lấy dữ liệu theo ID
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns></returns>
        Task<TDto?> GetById(long id);

        /// <summary>
        /// Submit dữ liệu (Add/Edit/Remove theo EntityState)
        /// </summary>
        /// <param name="request">Dữ liệu với EntityState</param>
        /// <returns></returns>
        Task<BaseResponse> Submit(TDtoEdit request);
    }
}