namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Discount_code
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Discount_code()
        {
            Order = new HashSet<Order>();
            Voucher_user = new HashSet<Voucher_user>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_discount { get; set; }

        [Required]
        public string code { get; set; }

        public int sale { get; set; }

        public int? stock { get; set; }

        [StringLength(10)]
        public string hide { get; set; }

        [StringLength(50)]
        public string start_date { get; set; }

        [StringLength(50)]
        public string end_date { get; set; }

        [StringLength(50)]
        public string type_voucher { get; set; }

        [StringLength(50)]
        public string rank_user { get; set; }

        public string info { get; set; }

        public string name_code { get; set; }

        public string photo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Voucher_user> Voucher_user { get; set; }
    }
}
