using System.ComponentModel.DataAnnotations;

namespace BackendWebCoreApi.Models
{
    public class Order
    {
        [Key]
        public int id { get; set; }
        public DateTime CreatedDate { set; get; }

        public virtual ICollection<OrderItem>? ordersItems { get; set; }
    }
}
