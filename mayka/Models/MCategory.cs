using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Models
{
    public class MCategory
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public virtual List<MProduct> Products { get; set; }

        public MCategory()
        {
            Products = new List<MProduct>();
        }
    }
}
