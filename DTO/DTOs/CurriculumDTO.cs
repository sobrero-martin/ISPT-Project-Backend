using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs
{
    public class CurriculumDTO
    {
        public long Id { get; set; }
        public required string Resolution { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime VigencyDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
