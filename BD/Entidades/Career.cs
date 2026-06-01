using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Career : BaseEntity
    {
        public required string Name { get; set; }

        public required string Title { get; set; }
    }
}
