using DTO.DTOs.CareerDTO;
using DTO.ENUM;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CoursesDTO.InscriptionForm
{
    public class SubjectsInscriptionDTO
    {
        public string SubjectName { get; set; }
        public int YearAvailable { get; set; }
        public SubjectStudentStatus IsAvailable { get; set; }
        public List<DivisionInscriptionDTO> Divisions { get; set; }
    }
}
