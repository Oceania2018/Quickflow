using CustomEntityFoundation.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Quickflow.Core.Entities
{
    public class OptionsInActivity : Entity, IDbRecord
    {
        public override string ToString()
        {
            return $"{Key}: {Value}\r\n";
        }

        [Required]
        [StringLength(36)]
        public String WorkflowActivityId { get; set; }

        [Required]
        [MaxLength(64)]
        public String Key { get; set; }

        [MaxLength(256)]
        public String Value { get; set; }
    }
}
