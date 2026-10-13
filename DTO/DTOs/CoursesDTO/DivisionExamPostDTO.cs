using DTO.ENUM;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CoursesDTO
{
    public class DivisionExamPostDTO
    {
        public long? Id {  get; set; }
        public Guid? CreatedById { get; set; }
        public long DivisionId { get; set; }
        public DateTime Date { get; set; }
        public EnumExamTypes Type { get; set; }
        public int Number { get; set; }
    }
}
