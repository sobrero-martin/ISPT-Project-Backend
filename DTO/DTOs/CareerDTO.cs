using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs
{
    public class CareerDTO
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Title { get; set; }
    }
}
