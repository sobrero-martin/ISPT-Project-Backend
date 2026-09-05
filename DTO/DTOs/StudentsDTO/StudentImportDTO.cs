using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.StudentsDTO
{
    public class StudentImportDTO
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
