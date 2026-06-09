using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Correlative : BaseEntity
    {
        public long SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public long SubjectCorrelativeId { get; set; }
        public Subject? SubjectCorrelative { get; set; }
    }
}
