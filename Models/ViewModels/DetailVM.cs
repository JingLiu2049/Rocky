using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models.ViewModels
{
    public class DetailVM
    {
        public  DetailVM()
        {
            this.Product = new Product();
        }

        public Product Product { get; set; }
        public bool IsInCart { get; set; }

    }
}
