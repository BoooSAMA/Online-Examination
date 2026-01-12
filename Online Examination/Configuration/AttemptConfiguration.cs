using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Online_Examination.Domain;

namespace Online_Examination.Data.Configurations
{
    public class AttemptConfiguration : IEntityTypeConfiguration<Attempt>
    {
        public void Configure(EntityTypeBuilder<Attempt> builder)
        {
            // 配置关系：防止级联删除报错
            builder.HasOne(a => a.User)
                   .WithMany(u => u.Attempts)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Exam)
                   .WithMany(e => e.Attempts)
                   .HasForeignKey(a => a.ExamId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}