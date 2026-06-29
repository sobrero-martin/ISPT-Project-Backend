using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CareerDTO
{
    public class SubjectCorrelativesDTO
    {
        public long Id { get; set; }
        public required string Code { get; set; }

        public required string Name { get; set; }

        public required string Format { get; set; }

        public bool IsCorrelative { get; set; }
    }
}
