using Store.Domin.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domin.Enitity
{
    public class Payment : BaseEntity
    {
        public PaymentMethod Method { get; set; }
        public PaymentStatus status { get; set; }
        public double? Amount { get; set; }

        [ForeignKey("Orders")]
        public int OrderId { get; set; }

        public Orders Order { get; set; }
    }
}
