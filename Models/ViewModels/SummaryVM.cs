using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models.ViewModels
{
    public class SummaryVM
    {
        public List<Product> ProductList { get; set; }
        public AppUser User { get; set; }
    }
}
