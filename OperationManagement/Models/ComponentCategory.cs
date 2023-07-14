﻿using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationManagement.Models
{
    public class ComponentCategory
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required,ForeignKey("EnterpriseId")]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public List<Component>? Components { get; set; }
    }
}
