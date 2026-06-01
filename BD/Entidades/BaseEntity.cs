using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BD.Entidades
{
    public abstract class BaseEntity
    {
        [Key]
        public long Id { get; set; }
        
        [JsonIgnore]
        public bool state { get; set; }
        
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
        
        [JsonIgnore]
        public DateTime? UpdatedAt { get; set; }
        
        [JsonIgnore]
        public long CreatedBy { get; set; }
        
        [JsonIgnore]
        public long? UpdatedBy { get; set; }
    }
}