using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BD.Entidades
{
    public class File : BaseEntity
    {
        public long StudentId { get; set; }
        public Person Student { get; set; }
        
        public List<Documentation> Documentations { get; set; }
        [JsonIgnore]
        public List<Grade> Grades { get; set; }
        [JsonIgnore]
        public List<Attendance> Attendances { get; set; }
    }
}
