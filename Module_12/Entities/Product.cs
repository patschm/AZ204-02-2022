using System.Collections.Generic;

namespace Entities
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }    
        public int BrandID { get; set; } 
        public Brand Brand { get; set; }
        public ICollection<ProductGroupProduct> ProductGroups { get; set; } = new HashSet<ProductGroupProduct>();
    }
}
