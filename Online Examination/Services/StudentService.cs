using Microsoft.EntityFrameworkCore;
using Online_Examination.Domain;
using Online_Examination.Data;

namespace Online_Examination.Services
{
    public class StudentService
    {
        private readonly AppDbContext _context;

        // 构造函数注入数据库上下文
        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. 用户认证模块
        // ==========================================

        // 注册新学生
        public async Task<User> RegisterStudentAsync(User user)
        {
            // 简单检查邮箱是否已存在
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new Exception("该邮箱已被注册");
            }

            user.Role = "Student";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // 学生登录
        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            return user;
        }

        // ==========================================
        // 2. 考试查询模块
        // ==========================================

        // 获取所有可用的试卷
        public async Task<List<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams.ToListAsync();
        }

        // 获取单张试卷的详细内容（开始考试用）
        public async Task<Exam?> GetExamByIdAsync(int examId)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);
        }

        // ==========================================
        // 3. 核心业务：交卷 & 自动判分
        // ==========================================

        public async Task<Attempt> SubmitExamAsync(int userId, int examId, Dictionary<int, string> studentAnswers)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) throw new Exception("试卷不存在");

            int finalScore = 0;

            foreach (var question in exam.Questions)
            {
                if (studentAnswers.ContainsKey(question.Id))
                {
                    string myAnswer = studentAnswers[question.Id];

                    if (string.Equals(myAnswer, question.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
                    {
                        finalScore++;
                    }
                }
            }

            var attempt = new Attempt
            {
                UserId = userId,
                ExamId = examId,
                Score = finalScore,
            };

            _context.Attempts.Add(attempt);
            await _context.SaveChangesAsync();

            return attempt;
        }

        // ==========================================
        // 4. 历史记录模块
        // ==========================================

        public async Task<List<Attempt>> GetStudentHistoryAsync(int userId)
        {
            return await _context.Attempts
                .Where(a => a.UserId == userId)
                .Include(a => a.Exam)
                .OrderByDescending(a => a.DateCreated)
                .ToListAsync();
        }
    }
}