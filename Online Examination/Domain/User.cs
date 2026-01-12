using System.ComponentModel.DataAnnotations;

namespace Online_Examination.Domain
{
    public class User : BaseDomainModel
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // 核心字段：角色
        // 我添加了 [MaxLength] 属性，这样数据库里不会是 nvarchar(max)，性能更好
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Student"; // 默认是 Student

        // --- 密码找回字段 ---
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // --- 导航属性 (EF Core 关系映射) ---

        // 1. 如果这个用户是“老师/管理员”，这是他创建的试卷列表
        public List<Exam> CreatedExams { get; set; } = new List<Exam>();

        // 2. 如果这个用户是“学生”，这是他参加过的考试尝试记录
        public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    }
}