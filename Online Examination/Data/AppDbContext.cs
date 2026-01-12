using Microsoft.EntityFrameworkCore;
using Online_Examination.Domain;
using System.Reflection; // 必须引用：为了使用 Assembly

namespace Online_Examination.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // 声明表
        public DbSet<User> Users { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Attempt> Attempts { get; set; }

        // 自动审计功能 (保持不变)
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseDomainModel>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DateCreated = DateTime.Now;
                    entry.Entity.DateUpdated = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.DateUpdated = DateTime.Now;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        // 配置表关系
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✨ 见证奇迹的一行代码 ✨
            // 这行代码会自动扫描 Data/Configurations 文件夹下所有的配置类并应用它们
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}