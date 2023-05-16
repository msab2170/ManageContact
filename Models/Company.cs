using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddressManager.Models
{
    [PrimaryKey(nameof(Id))]
    public class Company
    {
        public int Id { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "이름을 입력하세요.")]
        [DisplayName("Company Name")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "주소를 입력하세요.")]
        public string Address { get; set; }

        [MinLength(4), MaxLength(12)]
        [Required(ErrorMessage = "연락처를 입력하세요.")]
        [Phone]
        public string Contact { get; set; }

        [MaxLength(1)]
        public string IsDelete { get; set; } = "N";

        public ICollection<Worker>? Workers { get; set; }
    }
}
