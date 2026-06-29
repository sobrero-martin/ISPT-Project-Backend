using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CareerDTO
{
    public class SubjectPostDTO
    {
        public long Id { get; set; }
        public required long CurriculumId { get; set; }
        public required string Code { get; set; }
        public int Year { get; set; }
        public required string Name { get; set; }

        public required string Format { get; set; }

        public required string Type { get; set; }

        public int Duration { get; set; }
    }
}
