using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.SchoolYearDTO
{
    public class StudentFileDivisionPostDTO
    {
        public int FileId { get; set; } 
        public int DivisionId { get; set; }
        public int SchoolYearId { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
