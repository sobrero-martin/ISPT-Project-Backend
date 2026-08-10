using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Attendance : BaseEntity
    {
        public long FileDivisionId { get; set; }
        public FileDivision FileDivision { get; set; }
        
        public long AttendanceDayId { get; set; }
        public AttendanceDay AttendanceDay { get; set; }
        
        public bool Status { get; set; }
    }
}
