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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_cart { get; set; }

        public int id_user { get; set; }

        public int id_product { get; set; }

        public int? quantity_cart { get; set; }

        public string color_pro { get; set; }

        public string size_pro { get; set; }

        public virtual product product { get; set; }

        public virtual User_info User_info { get; set; }
    }
}
