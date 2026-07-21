using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BD.Entidades
{
    [Index(nameof(CareerId), nameof(Resolution), IsUnique = true)]
    public class Curriculum : BaseEntity
    {
        public long CareerId { get; set; }
        public Career? Career { get; set; }

        public required string Resolution { get; set; }

        public DateTime VigencyDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Duration { get; set; }
    }
}
