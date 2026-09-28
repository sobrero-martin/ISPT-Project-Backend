using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.SchoolYearDTO
{
    public class SchoolYearCurriculumDTO
    {
        public long Id { get; set; }
        public int AcademicYear { get; set; }
        public string Resolution { get; set; } = string.Empty;
    }
}
