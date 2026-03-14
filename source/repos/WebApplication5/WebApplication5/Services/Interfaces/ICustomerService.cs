using Microsoft.AspNetCore.Mvc;
using WebApplication5.DTOs.Customer;
using WebApplication5.DTOs.Invoice;
using WebApplication5.DTOs.Stats;
using WebApplication5.Models;

namespace WebApplication5.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto);
        Task<IEnumerable<CustomerResponseDto>> GetAllAsync();
        Task<CustomerResponseDto?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> ArchiveAsync(int id);
        Task<CustomerResponseDto?> UpdateAsync(int id, CreateCustomerDto dto);
        Task<IEnumerable<CustomerResponseDto>> GetPagedAsync(int page,
            int pageSize,
            string sortBy,
            string sortOrder,
            string? search=null,string? by=null);
        Task<IEnumerable<CustomerStatsDto>> GetCustomerStats(DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
