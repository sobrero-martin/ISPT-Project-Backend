using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BD.Entidades
{
    [Index(nameof(CareerId), nameof(Resolution), IsUnique = true)]
    public class Curriculum : BaseEntity
    {
        public long CareerId { get; set; }
        public Career? Career { get; set; }

        public required string Resolution { get; set; }

        public DateTime VigencyDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Range(1, 500, ErrorMessage = "La duración debe ser un valor entre 1 y 500")]
        public int Duration { get; set; }
    }
}
