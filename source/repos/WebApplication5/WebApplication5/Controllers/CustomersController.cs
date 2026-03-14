using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.DTOs.Customer;
using WebApplication5.Services;
using WebApplication5.Services.Interfaces;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAll() {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponseDto>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer is null)
                return NotFound($"Project with ID {id} not found");
            return Ok(customer);
        }
        [HttpPost] 
        public async Task<ActionResult<CustomerResponseDto>> Create(CreateCustomerDto dto)
        {
            var customer = await _customerService.CreateCustomerAsync(dto);
            return Ok(customer);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerResponseDto?>> Update(int id,CreateCustomerDto dto)
        {
            var customer = await _customerService.UpdateAsync(id, dto);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }
        [HttpDelete("soft/{id}")]
        public async Task<ActionResult> Archive(int id)
        {
            var customer = await _customerService.ArchiveAsync(id);
            if (customer == false)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var customer = await _customerService.DeleteAsync(id);
            if (customer == false)
                return NotFound();
            return NoContent();
        }
        [HttpGet("Paged")]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetPaged(int page = 1,
    int pageSize = 10,
    string sortBy = "CreatedAt",
    string sortOrder = "desc",
    string? search = null,string? by=null)
        {
            var customers = await _customerService.GetPagedAsync(page,pageSize,sortBy,sortOrder,search,by);
            return Ok(customers);
        }
        [HttpGet("Stats")]
        public async Task<IActionResult> CustomerStats(
    DateTimeOffset startDate,
    DateTimeOffset endDate)
        {
            var result = await _customerService.GetCustomerStats(startDate, endDate);
            return Ok(result);
        }
    }
}
