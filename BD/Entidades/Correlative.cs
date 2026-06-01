using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Correlative : BaseEntity
    {
        public long SubjectID { get; set; }
        public Subject? Subject { get; set; }

        public long SubjectCorrelativeID { get; set; }
        public Subject? SubjectCorrelative { get; set; }
    }
}
