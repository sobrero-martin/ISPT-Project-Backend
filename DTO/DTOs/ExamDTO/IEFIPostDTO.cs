namespace DTO.DTOs.ExamDTO
{
    public class IEFIPostDTO
    {
        public long Id { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public long DivisionId { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly Time { get; set; }
        public int? RecordBook { get; set; }
        public int? PageNumber { get; set; }
    }
}
