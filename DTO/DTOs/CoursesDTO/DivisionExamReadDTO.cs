using DTO.ENUM;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CoursesDTO
{
    public class DivisionExamReadDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int Number { get; set; }
    }
}
