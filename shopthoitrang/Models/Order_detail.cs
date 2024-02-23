namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order_detail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_orderdetail { get; set; }

        public int id_order { get; set; }

        public int? id_product { get; set; }

        public string size_pro { get; set; }

        public string color_pro { get; set; }

        public int? quantity_pro { get; set; }

        public virtual Order Order { get; set; }

        public virtual product product { get; set; }
    }
}
