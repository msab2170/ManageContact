using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AddressManager.Models
{
    [PrimaryKey(nameof(Id))]
    public class Worker
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }

        public Company Company { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "이름을 입력하세요.")]
        [DisplayName("Worker Name")]
        public string Name { get; set; }

        [MaxLength(20), EmailAddress]
        public string Email { get; set; }

        [MinLength(4), MaxLength(12)]
        [Required(ErrorMessage = "연락처를 입력하세요.")]
        public string Phone { get; set; }

        [MaxLength(1)]
        public string IsDelete { get; set; } = "N";

    }
}
