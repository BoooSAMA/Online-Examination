namespace Online_Examination.Domain // ⚠️注意：请将 YourApp 替换为你项目实际的命名空间
{
    public abstract class BaseDomainModel
    {
        public int Id { get; set; }

        // 优化：赋予默认值，省心
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}