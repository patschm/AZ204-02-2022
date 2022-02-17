using System.Collections.Generic;

namespace Entities
{
    public class ProductGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<ProductGroupProduct> Products { get; set; } = new HashSet<ProductGroupProduct>();
    }
}
