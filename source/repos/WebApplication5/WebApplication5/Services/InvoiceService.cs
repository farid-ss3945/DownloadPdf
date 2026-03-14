using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.DTOs;
using WebApplication5.DTOs.Customer;
using WebApplication5.DTOs.Invoice;
using WebApplication5.DTOs.Stats;
using WebApplication5.Models;
using WebApplication5.Services.Interfaces;

namespace WebApplication5.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly InvoiceManagerDbContext _context;
        private readonly IMapper _mapper;
        public InvoiceService(InvoiceManagerDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        private static decimal CalculateTotal(IEnumerable<InvoiceRow> rows) =>
    rows.Sum(r => r.Quantity * r.Rate);

        public async Task<bool> ArchiveAsync(int id)
        {
            var invoice = await _context.Invoices
            .Include(i=>i.Rows)
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

            if (invoice == null)
                return false;

            invoice.DeletedAt = DateTimeOffset.UtcNow;
            invoice.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<InvoiceResponseDto> ChangeStatusAsync(int id, string status)
        {
            var invoice = await _context.Invoices.Include(i=>i.Rows).FirstOrDefaultAsync(i=>i.Id==id);
            if (invoice == null) return null;
            if (status == null) return null;
            //if (status == "Created")
            //{
            //    invoice.UpdatedAt = DateTimeOffset.UtcNow;
            //    await _context.SaveChangesAsync();
            //    return _mapper.Map<InvoiceResponseDto>(invoice);
            //}
            //else if (status == "Sent")
            //{
            //    invoice.Status = Models.InvoiceStatus.Sent;
            //    invoice.UpdatedAt = DateTimeOffset.UtcNow;
            //    await _context.SaveChangesAsync();
            //    return _mapper.Map<InvoiceResponseDto>(invoice);
            //}
            //else if (status == "Received")
            //{
            //    invoice.Status = Models.InvoiceStatus.Received;
            //    invoice.UpdatedAt = DateTimeOffset.UtcNow;
            //    await _context.SaveChangesAsync();
            //    return _mapper.Map<InvoiceResponseDto>(invoice);
            //}
            //else if (status == "Paid")
            //{

            //    invoice.Status = Models.InvoiceStatus.Paid;
            //    invoice.UpdatedAt = DateTimeOffset.UtcNow;
            //    await _context.SaveChangesAsync();
            //    return _mapper.Map<InvoiceResponseDto>(invoice);
            //}
            //else if (status == "Cancelled")
            //{
            //    invoice.Status = Models.InvoiceStatus.Cancelled;
            //    invoice.UpdatedAt = DateTimeOffset.UtcNow;
            //    await _context.SaveChangesAsync();
            //    return _mapper.Map<InvoiceResponseDto>(invoice);
            //}
            //if (status == "Rejected")
            //{
            //    invoice.Status = Models.InvoiceStatus.Rejected;
            //    invoice.UpdatedAt = DateTimeOffset.UtcNow;
            //    await _context.SaveChangesAsync();
            //    return _mapper.Map<InvoiceResponseDto>(invoice);
            //}
            //else { 
            //    return null; 
            //}
            if (!Enum.TryParse<Models.InvoiceStatus>(status, ignoreCase: true, out var parsedStatus))
                return null; 

            invoice.Status = parsedStatus;
            invoice.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<InvoiceResponseDto>(invoice);

        }

        public async Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto)
        {
            var invoice = _mapper.Map<Invoice>(dto);
            invoice.Status=Models.InvoiceStatus.Created;
            invoice.CreatedAt= DateTimeOffset.UtcNow;
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
            {
                return null;

            }
            invoice.CustomerId=dto.CustomerId;
            //decimal sum=0;
            //foreach (var i in invoice.Rows)
            //{
            //    sum += (i.Quantity * i.Rate);
            //}
            invoice.TotalSum = CalculateTotal(invoice.Rows);

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            await
            _context.Entry(invoice)
            .Collection(p => p.Rows)
            .LoadAsync();
            return _mapper.Map<InvoiceResponseDto>(invoice);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var invoice = await _context.Invoices.Include(i => i.Rows).FirstOrDefaultAsync(i => i.Id == id);
            if (invoice == null) return false;
            if (invoice.Status == Models.InvoiceStatus.Sent)
            {
                return false;
            }
            _context.InvoiceRows.RemoveRange(invoice.Rows);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<InvoiceResponseDto>> GetAllAsync()
        {
            return await _context.Invoices
                .Where(c => c.DeletedAt == null)
                .Include(i => i.Rows)
                .Select(c => new InvoiceResponseDto
                {
                    Id = c.Id,
                    CustomerId = c.CustomerId,
                    Comment= c.Comment,
                    TotalSum= c.TotalSum,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status= (DTOs.Invoice.InvoiceStatus)c.Status,
                    Rows= c.Rows
                .Select(r => new InvoiceRowDto
                {
                    Service = r.Service,
                    Quantity = r.Quantity,
                    Rate = r.Rate
                })
                .ToList()
                })
            .ToListAsync();
        }

        public async Task<InvoiceResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Invoices
                .Where(c => c.Id == id && c.DeletedAt == null)
                .Include(i => i.Rows)
                .Select(c => new InvoiceResponseDto
                {
                    Id = c.Id,
                    CustomerId = c.CustomerId,
                    Comment = c.Comment,
                    TotalSum = c.TotalSum,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status = (DTOs.Invoice.InvoiceStatus)c.Status,
                    Rows = c.Rows
                .Select(r => new InvoiceRowDto
                {
                    Service = r.Service,
                    Quantity = r.Quantity,
                    Rate = r.Rate
                })
                .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<InvoiceResponseDto?> UpdateAsync(int id, CreateInvoiceDto dto)
        {
            var invoice = await _context.Invoices.Include(i=>i.Rows).FirstOrDefaultAsync(c => c.Id == id);
            if (invoice == null || invoice.DeletedAt != null)
            {
                return null;
            }
            if (invoice.Status == Models.InvoiceStatus.Sent)
            {
                return null;
            }
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
            {
                return null;

            }
            
            _mapper.Map(dto, invoice);
            invoice.UpdatedAt = DateTimeOffset.UtcNow;
            //decimal sum = 0;
            //foreach (var i in invoice.Rows)
            //{
            //    sum += (i.Quantity * i.Rate);
            //}
            invoice.TotalSum = CalculateTotal(invoice.Rows);

            await _context.SaveChangesAsync();
            return _mapper.Map<InvoiceResponseDto>(invoice);
        }
        public async Task<IEnumerable<InvoiceResponseDto>> GetPagedAsync(int page,
                                                                          int pageSize,
                                                                          string sortBy,
                                                                          string sortOrder,
                                                                          string? search=null)

        {
            var query = _context.Invoices.Where(i=>i.DeletedAt==null).AsQueryable();
            if (string.IsNullOrWhiteSpace(search) == false)
            {
                if (decimal.TryParse(search, out var totalSum))
                    query = query.Where(i => i.TotalSum == totalSum);

                else if (Enum.TryParse<Models.InvoiceStatus>(search, true, out var status))
                    query = query.Where(i => (int)i.Status == (int)status);

                else if (DateTime.TryParse(search, out var date))
                    query = query.Where(i => i.CreatedAt.Date == date.Date);
            }
            switch (sortBy)
            {
                case "Id":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.Id)
                    : query.OrderBy(i => i.Id);
                    break;
                case "CustomerId":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.CustomerId)
                    : query.OrderBy(i => i.CustomerId);
                    break;
                case "TotalSum":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.TotalSum)
                    : query.OrderBy(i => i.TotalSum);
                    break;
                case "Comment":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.Comment)
                    : query.OrderBy(i => i.Comment);
                    break;
                case "Status":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.Status)
                    : query.OrderBy(i => i.Status);
                    break;
                case "StartDate":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.StartDate)
                    : query.OrderBy(i => i.StartDate);
                    break;
                case "EndDate":
                    query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.EndDate)
                    : query.OrderBy(i => i.EndDate);
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
            var invoices = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<InvoiceResponseDto>>(invoices);
        }
        public async Task<IEnumerable<InvoiceStatsDto>> GetInvoiceStats(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _context.InvoiceRows
                    .Where(r => r.Invoice.CreatedAt >= startDate && r.Invoice.CreatedAt <= endDate)
                    .GroupBy(r => r.Service)
                    .Select(g => new InvoiceStatsDto
                    {
                        Service = g.Key,
                        InvoiceCount = g.Select(r => r.InvoiceId).Distinct().Count(),
                        TotalSum = g.Sum(r => r.Sum)
                    })
                    .ToListAsync();
        }
        public async Task<IEnumerable<InvoiceStatusStatsDto>> GetInvoiceStatusStats(
    DateTimeOffset startDate,
    DateTimeOffset endDate)
        {
            return await _context.Invoices
                .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
                .GroupBy(i => i.Status)
                .Select(g => new InvoiceStatusStatsDto
                {
                    Status = g.Key,
                    InvoiceCount = g.Count()
                })
                .ToListAsync();
        }
    }
}
