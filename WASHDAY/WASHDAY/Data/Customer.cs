using System.ComponentModel.DataAnnotations;
namespace WASHDAY_202508.Data
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        public string? Notes { get; set; } // 備註 (例如：喜歡摺衣服的方式)

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 建檔日期
    }
}
