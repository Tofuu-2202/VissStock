using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ServiceStack;

namespace VisssStock.Application.DTOs
{
    public class CreateUserDTO
    {
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
        [Required] [ValidateEmail] public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        [Phone]
        public string Phone { get; set; }
        public int IsTeacher { get; set; }
        [Required] public List<RoleRegisterUser> Roles { get; set; }
    }

    public class RoleRegisterUser
    {
        [Required] public int RoleId { get; set; }
        public List<int> SubjectId { get; set; }
    }
}