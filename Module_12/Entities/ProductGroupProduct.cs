namespace Entities
{
    public class ProductGroupProduct
    {
        public int ProductID { get; set; }
        public int ProductGroupID { get; set; }
        public Product Product { get; set; }
        public ProductGroup ProductGroup { get; set; }
    }
}
