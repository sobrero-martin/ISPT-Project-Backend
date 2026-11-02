using DTO.ENUM;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CoursesDTO
{
    public class CareerInscriptionDTO
    {
        public long CurriculumId { get; set; }
        public string CareerName { get; set; }
        public string VigencyDate { get; set; }
        public int Duration { get; set; }
        public string Title { get; set; }
        public int SchoolYearNumber { get; set; }
    }
}
