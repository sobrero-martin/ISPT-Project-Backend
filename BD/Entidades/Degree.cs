using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Degree : BaseEntity
    {
        public long PersonId { get; set; }
        public Person Person { get; set; }

        public string TitleName { get; set; }
    }
}
