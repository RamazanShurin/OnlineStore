using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mayka.Models;

namespace mayka.ViewModels
{
    public class CategoriesProductsVM
    {
        public List<MCategory> Categories { get; set; }
        public List<MProduct> Products { get; set; }
    }
}
