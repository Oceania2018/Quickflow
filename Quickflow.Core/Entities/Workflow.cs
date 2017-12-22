using CustomEntityFoundation.Entities;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Quickflow.Core.Entities
{
    public class Workflow : Entity, IDbRecord
    {
        public override string ToString()
        {
            return $"{Name} {Id}";
        }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public String Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public String Description { get; set; }

        [ForeignKey("WorkflowId")]
        public List<ActivityInWorkflow> Activities { get; set; }

        [StringLength(36)]
        [Required]
        public String RootActivityId { get; set; }
    }
}
