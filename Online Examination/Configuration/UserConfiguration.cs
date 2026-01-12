using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Online_Examination.Domain;

namespace Online_Examination.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // 1. 约束配置
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Role).HasMaxLength(20); // 推荐加上长度限制
            builder.Property(u => u.Username).HasMaxLength(50);

            // 2. 种子数据 (搬运过来的)
            builder.HasData(
                // 1. 管理员账号
                new User
                {
                    Id = 1,
                    Username = "Admin",
                    Email = "admin@school.com",
                    Password = "password123",
                    Role = "Admin",
                    DateCreated = DateTime.Parse("2024-01-01"),
                    DateUpdated = DateTime.Parse("2024-01-01"),
                    CreatedBy = "System",
                    UpdatedBy = "System"
                },
                // 2. 学生测试账号
                new User
                {
                    Id = 2,
                    Username = "John",
                    Email = "student@school.com",
                    Password = "123456",
                    Role = "Student",
                    DateCreated = DateTime.Parse("2024-01-01"),
                    DateUpdated = DateTime.Parse("2024-01-01"),
                    CreatedBy = "System",
                    UpdatedBy = "System"
                }
            );
        }
    }
}