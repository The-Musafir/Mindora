using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class WellnessResource
    {
        public Guid ResourceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsPublished { get; set; } = false;
        public virtual ICollection<WellnessResourceCategoryMapping> CategoryMappings { get; set; } = new List<WellnessResourceCategoryMapping>();
    }
}
