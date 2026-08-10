using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Grade : BaseEntity
    {
        public long FileDivisionId { get; set; }
        public FileDivision FileDivision { get; set; }
        
        public long DivisionExamId  { get; set; }
        public DivisionExam DivisionExam { get; set; }
        
        public short GradeNumber { get; set; }
    }
}
