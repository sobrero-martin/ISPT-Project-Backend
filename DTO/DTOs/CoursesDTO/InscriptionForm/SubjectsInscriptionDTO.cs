using DTO.DTOs.CareerDTO;
using DTO.ENUM;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CoursesDTO.InscriptionForm
{
    public class SubjectsInscriptionDTO
    {
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int YearAvailable { get; set; }
        public EnumInscriptionSubjectStudentStatus IsAvailable { get; set; }
        public List<string> MissingCorrelatives { get; set; } = new();
        public List<DivisionInscriptionDTO> Divisions { get; set; }
    }
}
