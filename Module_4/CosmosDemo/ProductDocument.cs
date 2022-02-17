using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities
{
    public class ProductDocument
    {
        public class ProductGroupDocument
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }
        public class BrandDocument
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public BrandDocument Brand { get; set; }
        public List<ProductGroupDocument> ProductGroups { get; set; }

        public override string ToString()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine($"Product: {Brand?.Name} {Name}");
            foreach (var o in ProductGroups)
            {
                bld.AppendLine($"\tIn productgroup: {o.Name}");
            }
            return bld.ToString();
        }
    }
}
