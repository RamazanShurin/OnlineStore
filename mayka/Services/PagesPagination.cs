using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Services
{
    public class PagesPagination
    {
        public int[] ShowPages(int page, decimal total)
        {
            int pageEnd = 3;
            int pageBegin = 1;
            int[] pages = new int[2];

            if (page > 2)
            {
                pageBegin = page - 2;
            }

            if (pageEnd + page < total)
            {
                pageEnd = pageEnd + page;
            }
            else
            {
                pageEnd = (int)total;
            }

            pages[0] = pageBegin;
            pages[1] = pageEnd;
            return pages;
        }
    }
}
