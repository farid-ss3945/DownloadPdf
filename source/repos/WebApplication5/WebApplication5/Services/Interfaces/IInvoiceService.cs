using WebApplication5.DTOs.Invoice;
using WebApplication5.DTOs.Stats;

namespace WebApplication5.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto);
        Task<IEnumerable<InvoiceResponseDto>> GetAllAsync();
        Task<InvoiceResponseDto?> GetByIdAsync(int id);
        Task<InvoiceResponseDto?> UpdateAsync(int id, CreateInvoiceDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ArchiveAsync(int id);
        Task<InvoiceResponseDto> ChangeStatusAsync(int id, string status);
        Task<IEnumerable<InvoiceResponseDto>> GetPagedAsync(int page,
                                                                          int pageSize,
                                                                          string sortBy,
                                                                          string sortOrder,
                                                                          string? search = null);
        Task<IEnumerable<InvoiceStatsDto>> GetInvoiceStats(DateTimeOffset startDate, DateTimeOffset endDate);
        Task<IEnumerable<InvoiceStatusStatsDto>> GetInvoiceStatusStats(
    DateTimeOffset startDate,
    DateTimeOffset endDate);
    }
}
