using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.ExamDTO
{
    public class FinalExamPostDTO
    {
        public long Id { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public long SubjectId { get; set; }
        public long PersonId { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly Time { get; set; }
        public int RecordBook { get; set; }
        public int PageNumber { get; set; }
    }
}
