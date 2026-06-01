using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Curriculum : BaseEntity
    {
        public long CareerID { get; set; }
        public Career? Career { get; set; }

        public required string Resolution { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime VigencyDate { get; set; }

        public int Duration { get; set; }
    }
}
