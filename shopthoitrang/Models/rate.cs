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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_rate { get; set; }

        public int? star_rate { get; set; }

        public int? id_product { get; set; }

        public int? id_comment { get; set; }

        public virtual product product { get; set; }
    }
}
