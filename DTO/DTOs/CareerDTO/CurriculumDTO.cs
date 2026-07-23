using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DTO.DTOs.CareerDTO
{
    public class CurriculumDTO
    {
        public long Id { get; set; }
        public required string Resolution { get; set; }
        [Range(1, 500, ErrorMessage = "La duración debe ser un valor entre 1 y 500")]
        public int Duration { get; set; }
        public DateTime VigencyDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
