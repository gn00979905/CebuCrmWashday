using CebuCrmApi.Data;
using CebuCrmApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace CebuCrmApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PmsController : Controller
    {
        private readonly CrmDbContext _context;
        public PmsController(CrmDbContext context)
        {
            _context = context;
        }
        [HttpGet("rooms")]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.ToListAsync();
        }

        [HttpPost("rooms")]
        public async Task<ActionResult<Room>> CreateRoom(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return Ok(room);
        }
        // --- 訂單管理 (Bookings) ---

        // 取得特定日期範圍內的訂單 (供時間軸使用)
        [HttpGet("bookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            // 搜尋邏輯：訂單的入住日小於查詢結束日，且退房日大於查詢起始日 (即有重疊)
            var bookings = await _context.Bookings
                .Where(b => b.CheckInDate < end && b.CheckOutDate > start)
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpPost("bookings")]
        public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
        {
            // 簡單的防撞檢查
            var overlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.CheckInDate < booking.CheckOutDate &&
                b.CheckOutDate > booking.CheckInDate);

            if (overlap)
            {
                return BadRequest("該時段房間已被預訂");
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(booking);
        }
    }
}
