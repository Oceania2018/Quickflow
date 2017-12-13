using CustomEntityFoundation.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Quickflow.Core.Entities
{
    public class ActivityInWorkflow : Entity, IDbRecord
    {
        public override string ToString()
        {
            return $"{ActivityName} {Id}";
        }

        [Required]
        [StringLength(36)]
        public string WorkflowId { get; set; }
        public Workflow Workflow { get; set; }

        [Required]
        [MaxLength(64)]
        public String ActivityName { get; set; }

        /// <summary>
        /// NextWorkflowActivityId
        /// </summary>
        [StringLength(36)]
        public String NextActivityId { get; set; }

        [ForeignKey("WorkflowActivityId")]
        public List<OptionsInActivity> Options { get; set; }

        /// <summary>
        /// Convert to options
        /// </summary>
        [NotMapped]
        public JObject Configuration { get; set; }

        [NotMapped]
        public Object Input { get; set; }

        [NotMapped]
        public ActivityResult Output { get; set; }
    }
}
