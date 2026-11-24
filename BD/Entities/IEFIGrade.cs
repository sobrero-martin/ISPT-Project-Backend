using BD.Entidades;
using DTO.ENUM;
using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entities
{
    public class IEFIGrade
    {
        public long IEFIId{ get; set; }
        public IEFI? IEFI { get; set; }

        public long FileDivisionId { get; set; }
        public FileDivision? FileDivision { get; set; }

        public int? Grade { get; set; }

        public EnumFinalGradeForIEFIandFExam? GradeState { get; set; } // Aprobado, Desaprobado, No asistió
    }
}
