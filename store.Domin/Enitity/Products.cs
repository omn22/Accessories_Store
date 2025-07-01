using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domin.Enitity
{
    public class Products : BaseEntity
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public List<string> ImageUrl { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
