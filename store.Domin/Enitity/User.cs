using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domin.Enitity
{
    public class User :IdentityUser<int>
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        /* Navigational properties */
        [NotMapped]
        public ICollection<Orders> Orders { get; set; } = new List<Orders>();

    }
}
