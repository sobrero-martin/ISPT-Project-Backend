using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace BD.Entidades
{
    [Index(nameof(CUIL), IsUnique = true)]
    public class Person : BaseEntity
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        
        public string CUIL { get; set; }
        public string TypeDocument  { get; set; }
        public string DocumentNumber { get; set; }
        
        public DateTime Birthdate { get; set; }
        public string PracticePlace { get; set; }
        public string Observations { get; set; }
        
        public Contact Contact { get; set; }
        public Location Location { get; set; }
        public List<Documentation> Documentations { get; set; }
        public List<Title> Titles { get; set; }
        
        // (LEGAJOS)
        [JsonIgnore]
        public List<File> Files { get; set; }
        [JsonIgnore]
        public List<TeacherDivision> TeacherDivisions { get; set; }
    }
}