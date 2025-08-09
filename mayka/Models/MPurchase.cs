using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Models
{
    public class MPurchase
    {
        public int Id { get; set; }
        public DateTime DatePurchase { get; set; }
        public DateTime DateDelivery { get; set; }
        public bool Completed { get; set; }
        
        public int ClientId { get; set; }
        [ForeignKey(nameof(ClientId))]
        public virtual MClient Client { get; set; }

        public virtual List<MPurchaseProduct> PurProducts { get; set; }

        public MPurchase()
        {
            PurProducts = new List<MPurchaseProduct>();
        }
    }
}
