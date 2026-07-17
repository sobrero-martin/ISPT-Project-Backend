using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CareerDTO
{
    public class CorrelativeChangeDTO
    {
        public long SubjectCorrelativeId { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public bool IsCorrelative { get; set; }
    }
}
