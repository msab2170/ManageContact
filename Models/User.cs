using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AddressManager.Models
{
    [PrimaryKey(nameof(Id))]
    public class User
    {
        public int Id { get; set; }

        [MinLength(4), MaxLength(20)]
        [DisplayName("User Id"), Required(ErrorMessage = "아이디를 입력하세요.")]
        public string LoginId { get; set; }

        [MinLength(4), MaxLength(20), DataType(DataType.Password)]
        [Required(ErrorMessage = "비밀번호를 입력하세요.")]
        public string Password { get; set; }
    }
}
