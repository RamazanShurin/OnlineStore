using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Models
{
    public class MPurchaseProduct
    {
        public int Id { get; set; }
        public string Size { get; set; }

        public string Title { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public string Currency { get; set; }
        public string Img { get; set; }
        public string ImgBack { get; set; }

        public int PurchaseId { get; set; }

        [ForeignKey(nameof(PurchaseId))]
        public MPurchase Purchase { get; set; }
    }
}
