using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class FinalExamGrade : BaseEntity
    {
        public long FinalExamId { get; set; }
        public FinalExam? FinalExam { get; set; }

        public long FileId { get; set; }
        public File? File { get; set; }

        public int Grade { get; set; }

        public required string GradeState { get; set; }
    }
}
