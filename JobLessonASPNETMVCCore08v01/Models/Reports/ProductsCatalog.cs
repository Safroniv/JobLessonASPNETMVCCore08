using Orders.DAL.Entities;

namespace JobLessonASPNETMVCCore08v01.Models.Reports
{
    public class ProductsCatalog
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime CreationDate { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}
