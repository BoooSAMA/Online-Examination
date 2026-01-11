using System.ComponentModel.DataAnnotations;

namespace Online_Examination.Domain
{
    public class User : BaseDomainModel
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "Student";

        // --- 密码找回字段 ---
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // --- 导航属性 ---
        public List<Exam> CreatedExams { get; set; } = new List<Exam>();
        public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    }
}
