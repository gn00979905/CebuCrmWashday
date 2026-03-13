using CebuCrmApi.Models;

namespace CebuCrmApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CrmDbContext context)
        {
            if (context.Rooms.Any()) return; // 如果已有資料就跳過

            var rooms = new List<Room>
            {
                new Room { RoomNumber = "101", Type = RoomType.Studio, PricePerNight = 2500, Status = RoomStatus.Clean },
                new Room { RoomNumber = "102", Type = RoomType.Studio, PricePerNight = 2500, Status = RoomStatus.Dirty },
                new Room { RoomNumber = "201", Type = RoomType.OneBedroom, PricePerNight = 4500, Status = RoomStatus.Clean },
                new Room { RoomNumber = "202", Type = RoomType.OneBedroom, PricePerNight = 4500, Status = RoomStatus.Maintenance },
                new Room { RoomNumber = "301", Type = RoomType.TwoBedroom, PricePerNight = 8000, Status = RoomStatus.Clean }
            };

            context.Rooms.AddRange(rooms);
            context.SaveChanges();

            var bookings = new List<Booking>
            {
                new Booking {
                    RoomId = rooms[0].Id,
                    GuestName = "John Doe",
                    CheckInDate = DateTime.Today.AddDays(-1),
                    CheckOutDate = DateTime.Today.AddDays(2),
                    Status = BookingStatus.CheckedIn,
                    TotalAmount = 7500
                },
                new Booking {
                    RoomId = rooms[2].Id,
                    GuestName = "Jane Smith",
                    CheckInDate = DateTime.Today.AddDays(1),
                    CheckOutDate = DateTime.Today.AddDays(4),
                    Status = BookingStatus.Confirmed,
                    TotalAmount = 13500
                }
            };

            context.Bookings.AddRange(bookings);
            context.SaveChanges();
        }
    }
}
