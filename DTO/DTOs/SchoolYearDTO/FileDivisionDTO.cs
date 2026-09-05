using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.SchoolYearDTO
{
    public class FileDivisionDTO
    {
        public required string FileCode { get; set; }
        public required string StudentName { get; set; }
        public required string FileDivisionStatus { get; set; }
        public long? FileDivisionObservations { get; set; }
    }
}
