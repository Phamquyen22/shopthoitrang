namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            Order_detail = new HashSet<Order_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_order { get; set; }

        public int id_user { get; set; }

        public int? id_discount { get; set; }

        public double? total { get; set; }

        [StringLength(50)]
        public string status_order { get; set; }

        [StringLength(50)]
        public string date_order { get; set; }

        [StringLength(50)]
        public string payment_status { get; set; }

        public virtual Discount_code Discount_code { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_detail> Order_detail { get; set; }

        public virtual User_info User_info { get; set; }
    }
}
