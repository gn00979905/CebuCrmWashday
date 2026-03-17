using CebuCrmApi.Data;
using CebuCrmApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace CebuCrmApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PmsController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public PmsController(CrmDbContext context)
        {
            _context = context;
        }

        // --- 房間管理 (Rooms) ---
        // 取得單一房間資訊
        [HttpGet("rooms/{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return room;
        }

        // 更新房間狀態 (Clean / Dirty / Maintenance)
        [HttpPut("rooms/{id}/status")]
        public async Task<IActionResult> UpdateRoomStatus(int id, [FromBody] int newStatus)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound("找不到該房間");
            }

            // 更新狀態
            // 假設 0 = Clean, 1 = Dirty, 2 = Maintenance
            room.Status = (RoomStatus)newStatus;

            // 標記為已修改並存檔
            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content，代表更新成功且不需回傳資料
        }

        // 輔助方法：檢查房間是否存在
        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
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
            // 1. 防撞檢查：確保該房間在該時段沒有被預訂
            var overlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.Status != BookingStatus.Cancelled && // 排除已取消的訂單
                b.CheckInDate < booking.CheckOutDate &&
                b.CheckOutDate > booking.CheckInDate);

            if (overlap)
            {
                return BadRequest("該時段房間已被預訂 (Room is already booked for these dates)");
            }

            // 2. CRM 整合：尋找或建立客戶資料
            // 嘗試透過名字尋找現有客戶 (實務上可能會用電話或 Email 判斷更準確)
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Name.ToLower() == booking.GuestName.ToLower());

            int clientId;

            if (existingClient != null)
            {
                clientId = existingClient.Id; // 找到舊客，直接使用他的 ID
            }
            else
            {
                // 找不到，自動幫他在 CRM 裡建一筆新客戶 (Lead)
                var newClient = new Client
                {
                    Name = booking.GuestName,
                    Source = "Airbnb/Walk-in", // 標記來源
                    Status = "New",
                    Interest = $"Booked Room {booking.RoomId}",//, // 記錄他的興趣
                    CreatedAt = DateTime.UtcNow // 給一個建立時間
                };
                _context.Clients.Add(newClient);
                await _context.SaveChangesAsync(); // 先存檔以取得自動產生的 ID

                clientId = newClient.Id;
            }

            // 將這筆訂單關聯到正確的客戶 ID
            booking.CustomerId = clientId;
            booking.CreatedAt = DateTime.UtcNow;

            // 3. 儲存訂單
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // 4. Service 整合：自動產生退房日的打掃任務
            // 假設你有一個 ServiceOrder 的 Model
            var cleaningJob = new ServiceOrder
            {
                ClientName = booking.GuestName,
                ServiceType = "Airbnb Cleaning", // 這是你在前端設定的選項之一
                Status = "Pending", // 待處理
                Amount = 0, // 可以預設一個清潔費，或是留空由阿姨填寫
                OrderDate = booking.CheckOutDate, // 設定在退房那天打掃
                Notes = $"Auto-generated cleaning task for Room {booking.RoomId} after check-out."
            };

            _context.ServiceOrders.Add(cleaningJob);
            await _context.SaveChangesAsync();

            return Ok(booking);
        }
    }
}