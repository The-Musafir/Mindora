using System;

namespace Mindora.Domain.Entities
{
    public class WellnessResourceCategoryMapping
    {
        public Guid ResourceId { get; set; }
        public Guid CategoryId { get; set; }
        public virtual WellnessResource Resource { get; set; } = null!;
        public virtual WellnessResourceCategory Category { get; set; } = null!;
    }
}
