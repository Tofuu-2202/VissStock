using System.ComponentModel.DataAnnotations;

namespace VisssStock.Application.Models
{
    public class ResetPasswordForm
    {
        [Required] public string currentPassword { get; set; }
        [Required] public string newPassword { get; set; }
    }
}
