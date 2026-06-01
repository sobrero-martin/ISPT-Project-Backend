using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BD.Entidades
{
    public class Schedule : BaseEntity
    {
        [Required(ErrorMessage = "Emaple name is required.")]
        [MaxLength(50, ErrorMessage = "The example name must not exceed {1} characters.")]
        public required string ExampleName { get; set; }

        public int DivisionID { get; set; }
        public Division? Division { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}
