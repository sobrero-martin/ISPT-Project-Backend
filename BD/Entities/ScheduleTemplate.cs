using BD.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entities
{
    public class ScheduleTemplate
    {
        public long DivisionTemplateId { get; set; }
        public DivisionTemplate? DivisionTemplate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Day { get; set; }
    }
}
