using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CareerDTO
{
    public class SubjectDTO
    {
        public long Id { get; set; }
        public required string Code { get; set; }

        public required string Name { get; set; }

        public int Year { get; set; }

        public required string Format { get; set; }

        public required string Type { get; set; }

        public int Duration { get; set; }

        public bool IsCorrelative { get; set; }
    }
}
