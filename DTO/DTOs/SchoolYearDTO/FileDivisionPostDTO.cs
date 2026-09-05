using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.SchoolYearDTO
{
    public class FileDivisionPostDTO
    {
        public long FileCurriculumId { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public long DivisionId { get; set; }
        public required string FileDivisionStatus { get; set; }
        public long? FileDivisionObservations { get; set; }
    }
}
