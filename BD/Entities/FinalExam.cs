using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class FinalExam : BaseEntity
    {
        public long SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly Time { get; set; }

        public int? RecordBook  { get; set; }

        public int? PageNumber { get; set; }
    }
}
