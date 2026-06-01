using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class FileDivision : BaseEntity
    {
        public long FileID { get; set; }
        public File? File { get; set; }

        public long DivisionID { get; set; }
        public Division? Division { get; set; }

        public required string FileDivisionState { get; set; }

        public long? FileDivisionObservations { get; set; }
    }
}
