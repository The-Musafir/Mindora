using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class WellnessResourceCategory
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<WellnessResourceCategoryMapping> ResourceMappings { get; set; } = new List<WellnessResourceCategoryMapping>();
    }
}
