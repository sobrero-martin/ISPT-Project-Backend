using DTO.ENUM;
using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class DivisionExam : BaseEntity
    {
        public long DivisionId { get; set; }
        public Division? Division { get; set; }

        public DateTime Date { get; set; }

        public EnumExamTypes Type { get; set; }

        public int Number { get; set; }
    }
}
