using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.DTOs.Customer;
using WebApplication5.DTOs.Invoice;
using WebApplication5.Models;
using WebApplication5.Services;
using WebApplication5.Services.Interfaces;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetAll()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return Ok(invoices);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice is null)
                return NotFound($"Project with ID {id} not found");
            return Ok(invoice);
        }
        [HttpPost]
        public async Task<ActionResult<InvoiceResponseDto>> Create(CreateInvoiceDto dto)
        {
            var invoice = await _invoiceService.CreateAsync(dto);
            return Ok(invoice);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<InvoiceResponseDto?>> Update(int id, CreateInvoiceDto dto)
        {
            var invoice = await _invoiceService.UpdateAsync(id, dto);
            if (invoice == null)
                return NotFound();
            return Ok(invoice);
        }
        [HttpDelete("soft/{id}")]
        public async Task<ActionResult> Archive(int id)
        {
            var invoice = await _invoiceService.ArchiveAsync(id);
            if (invoice == false)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var invoice = await _invoiceService.DeleteAsync(id);
            if (invoice == false)
                return NotFound();
            return Ok("Deleted");
        }
        [HttpPut("{id}/{status}")]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var invoice = await _invoiceService.ChangeStatusAsync(id,status);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);

        }
        [HttpGet("Paged")]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetPaged(int page = 1,
    int pageSize = 10,
    string sortBy = "CreatedAt",
    string sortOrder = "desc")
        {
            var invoices = await _invoiceService.GetPagedAsync(page, pageSize, sortBy, sortOrder);
            return Ok(invoices);
        }
        [HttpGet("Download")]
        public async Task<IActionResult> Download(int id) { 
            var invoice = await _invoiceService.GetByIdAsync(id);
            var rowsHtml = string.Join("", invoice.Rows.Select(row =>
    $"<tr><td>Service : {row.Service}</td><td> Quantity : {row.Quantity}</td><td> Rate : {row.Rate}</td></tr>"
));
            var render=new ChromePdfRenderer();
            string html = $@"<p>Id : {invoice.Id}</p>
                            <p>CustomerId : {invoice.CustomerId}</p>
                            <p>Status : {invoice.Status}</p>
                            <p>StartDate : {invoice.StartDate}</p>
                            <p>EndDate : {invoice.EndDate}</p>
                            <p>TotalSum : {invoice.TotalSum}</p>
                            <p>Comment : {invoice.Comment}</p>
                            <p>{rowsHtml}</p>";
            var pdf=await render.RenderHtmlAsPdfAsync(html);
            var pdfBytes = pdf.BinaryData;

            return File(pdfBytes, "application/pdf", $"Invoice_{invoice.Id}.pdf");
        }
    }
}
