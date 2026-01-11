using System.ComponentModel.DataAnnotations;

namespace Online_Examination.Domain
{
    public class Attempt : BaseDomainModel
    {
        public int Score { get; set; }

        // --- 外键：学生 ---
        public int UserId { get; set; }
        public User? User { get; set; }

        // --- 外键：卷子 ---
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
    }
}
