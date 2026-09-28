using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.SchoolYearDTO
{
    public class SchoolYearByGradePostDTO
    {
        public long Id { get; set; }
        public Guid? CreatedById { get; set; }
        public long CurriculumYear1 { get; set; }
        public long CurriculumYear2 { get; set; }
        public long CurriculumYear3 { get; set; }
        public int SchoolYearNumber { get; set; }
    }
}
