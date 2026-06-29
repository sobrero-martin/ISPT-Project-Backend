using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.SchoolYearDTO
{
    public class SchoolYearPostDTO
    {
        public long Id { get; set; }
        public long CurriculumId { get; set; }
        public int SchoolYearNumber { get; set; }
    }
}
