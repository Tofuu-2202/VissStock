using System.ComponentModel.DataAnnotations;
using ServiceStack;

namespace VisssStock.Application.DTOs
{
    public class UpdateUserDTO
    {
        [Required] [ValidateEmail] public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        [Phone]
        public string Phone { get; set; }
        public int Enable { get; set; }
        [Required] public List<RoleRegisterUser> Roles { get; set; }
    }
}
