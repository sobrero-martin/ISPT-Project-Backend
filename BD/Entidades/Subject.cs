using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    [Index(nameof(Code), IsUnique = true)]
    public class Subject : BaseEntity
    {
        public long CurriculumID { get; set; }
        public Curriculum? Curriculum { get; set; }

        public required string Code { get; set; }

        public required string Name { get; set; }

        public int Year { get; set; }

        public required string Format { get; set; }

        public required string Type { get; set; }

        public int Duration { get; set; }
    }
}
