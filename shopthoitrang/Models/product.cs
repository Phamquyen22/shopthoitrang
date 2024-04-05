namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("product")]
    public partial class product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public product()
        {
            best_pro = new HashSet<best_pro>();
            Cart = new HashSet<Cart>();
            Order_detail = new HashSet<Order_detail>();
            rate = new HashSet<rate>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_product { get; set; }

        public int id_protype { get; set; }

        public int id_producer { get; set; }

        [Required]
        public string name_pro { get; set; }

        public string size_pro { get; set; }

        [StringLength(50)]
        public string color_pro { get; set; }

        public int? price_pro { get; set; }

        public int? discount_pro { get; set; }

        public string photo_pro { get; set; }

        [StringLength(50)]
        public string date_update { get; set; }

        public string info_pro { get; set; }

        public int? quantity_pro { get; set; }

        public string tag { get; set; }

        public int? buy_count { get; set; }

        [StringLength(50)]
        public string hide { get; set; }

        public string video_pro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<best_pro> best_pro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Cart { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_detail> Order_detail { get; set; }

        public virtual producer producer { get; set; }

        public virtual product_type product_type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rate> rate { get; set; }
    }
}
