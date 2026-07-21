using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CareerDTO
{
    public class CareerPostDTO
    {
        public long Id { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public required string Name { get; set; }
        public required string Title { get; set; }
    }
}
