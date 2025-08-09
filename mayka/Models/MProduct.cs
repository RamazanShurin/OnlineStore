using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Models
{
    public class MProduct
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Currency { get; set; }
        public string Img { get; set; }
        public string ImgBack { get; set; }
        public bool Public { get; set; }
        public int? Discount { get; set; }
        public string Color { get; set; }
        public string Composition { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public MCategory Category { get; set; }
    }
}
