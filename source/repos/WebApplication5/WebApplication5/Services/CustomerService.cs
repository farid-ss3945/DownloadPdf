using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.DTOs.Customer;
using WebApplication5.DTOs.Invoice;
using WebApplication5.DTOs.Stats;
using WebApplication5.Models;
using WebApplication5.Services.Interfaces;

namespace WebApplication5.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly InvoiceManagerDbContext _context;
        private readonly IMapper _mapper;
        public CustomerService(InvoiceManagerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        private static readonly Expression<Func<Customer, CustomerResponseDto>> ToDto = c => new CustomerResponseDto
        {
            Id = c.Id,
            UserId = c.UserId,
            Name = c.Name,
            Address = c.Address,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber
        };

        public async Task<bool> ArchiveAsync(int id)
        {
            var customer = await _context.Customers
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

            if (customer == null)
                return false;

            customer.DeletedAt = DateTimeOffset.UtcNow;
            customer.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return null;

            }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            await
            _context.Entry(customer)
            .Collection(p => p.Invoices)
            .LoadAsync();
            return _mapper.Map<CustomerResponseDto>(customer);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;
            var hasInvoices = await _context.Invoices.AnyAsync(i => i.CustomerId == id);
            if (hasInvoices) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync()
        {
            return await _context.Customers
                .Where(c => c.DeletedAt == null)
                .Select(ToDto)
            .ToListAsync();

        }

        public async Task<CustomerResponseDto?> GetByIdAsync(int id)
        {

            return await _context.Customers
                .Where(c => c.Id == id && c.DeletedAt == null)
                .Select(ToDto)
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerResponseDto?> UpdateAsync(int id, CreateCustomerDto dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null || customer.DeletedAt != null)
                return null;
            _mapper.Map(dto, customer);
            customer.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerResponseDto>(customer);

        }
        public async Task<IEnumerable<CustomerResponseDto>> GetPagedAsync(int page,
                                                                          int pageSize,
                                                                          string sortBy,
                                                                          string sortOrder,
                                                                          string? search = null, string? by = null)

        {

            var query = _context.Customers.Where(i => i.DeletedAt == null).AsQueryable();
            query = ApplySearch(query, search, by);

            switch (sortBy) {
                case "Id":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.Id)
                    : query.OrderBy(i => i.Id);
                    break;
                case "UserId":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.UserId)
                    : query.OrderBy(i => i.UserId);
                    break;
                case "Name":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.Name)
                    : query.OrderBy(i => i.Name);
                    break;
                case "Email":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.Email)
                    : query.OrderBy(i => i.Email);
                    break;
                case "Address":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.Address)
                    : query.OrderBy(i => i.Address);
                    break;
                case "PhoneNumber":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.PhoneNumber)
                    : query.OrderBy(i => i.PhoneNumber);
                    break;
                case "CreatedAt":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.CreatedAt)
                    : query.OrderBy(i => i.CreatedAt);
                    break;
                case "UpdatedAt":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.UpdatedAt)
                    : query.OrderBy(i => i.UpdatedAt);
                    break;
                case "DeletedAt":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.DeletedAt)
                    : query.OrderBy(i => i.DeletedAt);
                    break;
                default:
                    query = query.OrderByDescending(i => i.CreatedAt);
                    break;
            }

            pageSize = pageSize > 50 ? 50 : pageSize;
            page = page <= 0 ? 1 : page;
            var customers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }
        private static IQueryable<Customer> ApplySearch(IQueryable<Customer> query, string? search, string? by)
        {
            if (string.IsNullOrWhiteSpace(search)) return query;

            return by?.ToLower() switch
            {
                "name" => query.Where(c => c.Name == search),
                "email" => query.Where(c => c.Email == search),
                "phonenumber" => query.Where(c => c.PhoneNumber == search),
                _ => query.Where(c => c.Name == search || c.Email == search || c.PhoneNumber == search)
            };
        }
        public async Task<IEnumerable<CustomerStatsDto>> GetCustomerStats(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _context.Invoices
                  .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
                    .GroupBy(i => new { i.CustomerId, i.Customer.Name })
                    .Select(g => new CustomerStatsDto
                    {
                        CustomerId = g.Key.CustomerId,
                        CustomerName = g.Key.Name,
                        InvoiceCount = g.Count(),
                        TotalSum = g.Sum(i => i.TotalSum)
                    })
                    .ToListAsync();
        }

    }
}
