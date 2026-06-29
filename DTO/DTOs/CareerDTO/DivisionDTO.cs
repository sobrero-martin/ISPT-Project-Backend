using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CareerDTO
{
    public class DivisionDTO
    {
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public long SchoolYearId { get; set; }
        public required string Name { get; set; }
        public required string DivisionState { get; set; }
    }
}
