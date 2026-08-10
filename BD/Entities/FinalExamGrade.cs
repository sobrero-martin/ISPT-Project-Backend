using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class FinalExamGrade : BaseEntity
    {
        public long FinalExamId { get; set; }
        public FinalExam? FinalExam { get; set; }

        public long FileDivisionId { get; set; }
        public FileDivision? FileDivision { get; set; }

        public int Grade { get; set; }

        public required string GradeState { get; set; }
    }
}
