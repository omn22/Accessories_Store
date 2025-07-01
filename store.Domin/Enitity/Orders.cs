using Store.Domin.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domin.Enitity
{
    public class Orders : BaseEntity
    {
        public string UserPhoneNumer {  get; set; }
        public string userAddress { get; set; }

        public double TotalPrice {  get; set; }
        public int TotalQuantity { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime RecievedDate { get; set; }
        public OrderStatus Status { get; set; }

        [ForeignKey("User")]
        public int UserId {  get; set; }
        public User User { get; set; }
        [ForeignKey("Payment")]
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
        [NotMapped]
        public ICollection<Products> Products { get; set; } = new List<Products>();



    }
}
