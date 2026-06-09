using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class FinalExam : BaseEntity
    {
        public long CurriculumId { get; set; }
        public Curriculum? Curriculum { get; set; }

        public long PersonId { get; set; }
        public Person? Person { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly Time { get; set; }

        public int RecordBook  { get; set; }

        public int PageNumber { get; set; }
    }
}
