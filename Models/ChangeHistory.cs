using Microsoft.EntityFrameworkCore;

namespace AddressManager.Models
{
    [PrimaryKey(nameof(Id))]
    public class ChangeHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string LoginId { get; set; }

        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public int? WorkerId { get; set; }
        public string? WorkerName { get; set; }

        public string ActIP { get; set; }
        public string Act { get; set; }
        public DateTime ActDate { get; set; } = DateTime.Now;
    }
}
