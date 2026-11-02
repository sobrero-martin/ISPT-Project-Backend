using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CoursesDTO.InscriptionForm
{
    public class InscriptionFormDTO
    {
        public int YearsAvailable { get; set; }
        public List<SubjectsInscriptionDTO> Subjects { get; set; } 
    }
}
