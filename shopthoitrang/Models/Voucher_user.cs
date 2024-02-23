namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Voucher_user
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_voucher { get; set; }

        public int? id_discount { get; set; }

        public int? id_user { get; set; }

        [StringLength(50)]
        public string status_use { get; set; }

        [StringLength(50)]
        public string date_use { get; set; }

        public virtual Discount_code Discount_code { get; set; }

        public virtual User_info User_info { get; set; }
    }
}
