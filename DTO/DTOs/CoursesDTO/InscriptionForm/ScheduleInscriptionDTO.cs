using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.CoursesDTO.InscriptionForm
{
    public class ScheduleInscriptionDTO
    {
        public string Division { get; set; }
        public string Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

    }
}
