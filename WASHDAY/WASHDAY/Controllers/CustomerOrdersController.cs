using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WASHDAY_202508.Data;
using WASHDAY_202508.DTOs;

namespace WASHDAY_202508.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]// 允許前端 fetch 存取
    public class CustomerOrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CustomerOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/CustomerOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerOrderDto>>> GetCustomerOrders([FromQuery] int? year, [FromQuery] int? month)
        {
            var query = _context.CustomerOrders.AsQueryable();
            // **新增：** 如果提供了年份和月份，就篩選資料
            if (year.HasValue && month.HasValue)
            {
                query = query.Where(d => d.OrderDate.Year == year.Value && d.OrderDate.Month == month.Value);
            }
            return await query
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new CustomerOrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),// 在這裡格式化日期！
                    CustomerName = o.CustomerName,
                    OrderType = o.OrderType,
                    Description = o.Description,
                    PieceCount = o.PieceCount,
                    Amount = o.Amount,
                    IsPaid = o.IsPaid,
                    // **修正點：** 將一個 DateTime 拆分成兩個字串
                    PickupDate = o.PickupDateTime.HasValue ? o.PickupDateTime.Value.ToString("yyyy-MM-dd") : null,
                    PickupTime = o.PickupDateTime.HasValue ? o.PickupDateTime.Value.ToString("HH:mm") : null
                }).ToListAsync();
        }
        //POST: api/CustomerOrders
        [HttpPost]
        public async Task<ActionResult<CustomerOrderDto>> CreateCustomerOrder(CustomerOrderDto dto)
        {
            var order = new CustomerOrder
            {
                OrderDate = DateTime.Parse(dto.OrderDate),
                CustomerName = dto.CustomerName,
                OrderType = dto.OrderType,
                Description = dto.Description,
                PieceCount = dto.PieceCount,
                Amount = dto.Amount,
                IsPaid = dto.IsPaid,
                // **修正點：** 將兩個字串合併成一個 DateTime?
                PickupDateTime = CombineDateAndTime(dto.PickupDate, dto.PickupTime)
            };
            _context.CustomerOrders.Add(order);
            await _context.SaveChangesAsync();

            dto.Id = order.Id;
            return CreatedAtAction(nameof(GetCustomerOrders), new { id = order.Id }, dto);
        }
        //PUT: api/CustomerOrders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerOrder(int id, CustomerOrderDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var order = await _context.CustomerOrders.FindAsync(id);
            if(order == null)return NotFound();

            order.OrderDate = DateTime.Parse(dto.OrderDate);
            order.CustomerName = dto.CustomerName;
            order.OrderType = dto.OrderType;
            order.Description = dto.Description;
            order.PieceCount = dto.PieceCount;
            order.Amount = dto.Amount;
            order.IsPaid = dto.IsPaid;
            // **修正點：** 將兩個字串合併成一個 DateTime?
            order.PickupDateTime = CombineDateAndTime(dto.PickupDate, dto.PickupTime);

            await _context.SaveChangesAsync();
            return NoContent();
        }
        //DELETE: api/CustomerOrders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerOrder(int id)
        {
            var order = await _context.CustomerOrders.FindAsync(id);
            if (order == null) return NotFound();

            _context.CustomerOrders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // **新增：** 一個輔助方法來合併日期和時間
        private DateTime? CombineDateAndTime(string? date, string? time)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }

            // 如果有日期但沒有時間，預設為中午12點
            var timePart = string.IsNullOrEmpty(time) ? "12:00" : time;

            if (DateTime.TryParseExact(
                    $"{date} {timePart}",
                    "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var result))
            {
                return result;
            }

            return null; // 如果格式不符，回傳 null
        }
    }
}
