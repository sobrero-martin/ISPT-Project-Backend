using BD.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entities
{
    public class IEFI : BaseEntity
    {
        public long DivisionId { get; set; }
        public Division? Division { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly Time { get; set; }

        public int? RecordBook { get; set; }

        public int? PageNumber { get; set; }

    }
}
