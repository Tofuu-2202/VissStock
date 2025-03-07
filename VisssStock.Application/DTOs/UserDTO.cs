using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int IsTeacher { get; set; }
        public int Enable { get; set; }
        public string? TelegramChatId { get; set; }
        public string? TelegramUsername { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int IsDeleted { get; set; }
        public List<RoleDTOUser> roles { get; set; }
    }
    public class TelegramSaveRequestDTO
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string TelegramChatId { get; set; }
        public string TelegramUsername { get; set; }
    }

    public class RoleDTOUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 

    } 
}
