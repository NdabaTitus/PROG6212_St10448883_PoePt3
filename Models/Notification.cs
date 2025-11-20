using System.ComponentModel.DataAnnotations;

namespace Poe.Models
{
    public class Notification
    {
        public int Id { get; set; }


        [Required]
        [StringLength(200)]
        public string Message { get; set; }


        [Required]
        [StringLength(50)]
        public string TargetRole { get; set; }


        public bool IsRead { get; set; } = false;


        public int? WorkClaimId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

