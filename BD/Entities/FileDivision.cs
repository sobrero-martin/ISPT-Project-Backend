using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BD.Entidades
{
    public class FileDivision : BaseEntity
    { 
       public long FileCurriculumId { get; set; }
       public FileCurriculum FileCurriculum { get; set; }   

        public long DivisionId { get; set; }
        public Division? Division { get; set; }

        // Valida si se le admitio o no el cursado a esa division de ese espacio curricular
        public required string FileDivisionStatus { get; set; }

        public long? FileDivisionObservations { get; set; }
        
        [JsonIgnore]
        public List<Grade> Grades { get; set; }
        [JsonIgnore]
        public List<Attendance> Attendances { get; set; }
        [JsonIgnore]
        public List<FinalExamGrade> FinalExamGrades { get; set; }
    }
}
