using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CareerDTO
{
    public class CurriculumPostDTO
    {
        public long Id { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public required string Resolution { get; set; }
        public int Duration { get; set; }
        public DateTime VigencyDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long CareerId { get; set; }
    }
}
