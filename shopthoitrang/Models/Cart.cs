namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cart")]
    public partial class Cart
    {
        [Key]
        [StringLength(50)]
        public string id_cart { get; set; }

        [Required]
        [StringLength(50)]
        public string id_user { get; set; }

        [Required]
        [StringLength(50)]
        public string id_product { get; set; }

        public int? quantity_cart { get; set; }

        public string color_pro { get; set; }

        public string size_pro { get; set; }

        public virtual product product { get; set; }

        public virtual User_info User_info { get; set; }
    }
}
