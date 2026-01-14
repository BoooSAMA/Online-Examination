using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Online_Examination.Domain;
using Online_Examination.Data;

namespace Online_Examination.Services
{
    public class StudentService
    {
        private readonly Online_ExaminationContext _context;
        private readonly UserManager<Online_ExaminationUser> _userManager;
        private readonly SignInManager<Online_ExaminationUser> _signInManager;

        public StudentService(
            Online_ExaminationContext context,
            UserManager<Online_ExaminationUser> userManager,
            SignInManager<Online_ExaminationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ==========================================
        // 1. 用户认证模块
        // ==========================================

        public async Task<Online_ExaminationUser> RegisterStudentAsync(Online_ExaminationUser user, string password)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("邮箱地址不能为空");
            }

            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new Exception("该邮箱已被注册");
            }

            user.UserName = user.Email;

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"注册失败: {errors}");
            }

            return user;
        }

        public async Task<Online_ExaminationUser?> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }

        // ==========================================
        // 2. 考试查询模块
        // ==========================================

        public async Task<List<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams.ToListAsync();
        }

        public async Task<Exam?> GetExamByIdAsync(int examId)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);
        }

        // ==========================================
        // 3. 核心业务：交卷 & 自动判分
        // ==========================================

        public async Task<Attempt> SubmitExamAsync(string userId, int examId, Dictionary<int, string> studentAnswers)
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
                Score = finalScore
            };

            _context.Attempts.Add(attempt);
            await _context.SaveChangesAsync();

            return attempt;
        }

        // ==========================================
        // 4. 历史记录模块
        // ==========================================

        public async Task<List<Attempt>> GetStudentHistoryAsync(string userId)
        {
            return await _context.Attempts
                .Where(a => a.UserId == userId)
                .Include(a => a.Exam)
                .ToListAsync();
        }
    }
}