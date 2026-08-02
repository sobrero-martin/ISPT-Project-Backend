using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.ExamDTO
{
    public class FinalExamDTO
    {
        public long Id { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeOnly Time { get; set; }
        public int RecordBook { get; set; }
        public int PageNumber { get; set; }
    }
}
