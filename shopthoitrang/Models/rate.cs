namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rate")]
    public partial class rate
    {
        [Key]
        [StringLength(50)]
        public string Id_rate { get; set; }

        public int? star_rate { get; set; }

        [StringLength(50)]
        public string id_product { get; set; }

        [StringLength(50)]
        public string id_comment { get; set; }

        public virtual product product { get; set; }
    }
}
