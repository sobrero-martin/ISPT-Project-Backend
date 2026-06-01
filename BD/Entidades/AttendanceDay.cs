using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BD.Entidades
{
    public class AttendanceDay : BaseEntity
    {
        public DateTime Date { get; set; }
        public bool Status { get; set; }
        
        public long DivisionId { get; set; }
        public Division Division { get; set; }
        
        [JsonIgnore]
        public List<Attendance> Attendances { get; set; }
    }
}
