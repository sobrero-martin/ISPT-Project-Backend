using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using DTO.ENUM;
using Microsoft.EntityFrameworkCore;

namespace BD.Entidades
{
    [Index(nameof(Code), IsUnique = true)]
    public class File : BaseEntity
    {
        public string Code { get; set; }
        
        public long StudentId { get; set; }
        public Person Student { get; set; }
        
        public EnumStudentStatus Status { get; set; }
        
        public List<Documentation> Documentations { get; set; }
    }
}
