using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocky.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Rocky.Models.ViewModels;

namespace Rocky.Controllers
{
    [Authorize]
    public class ShoppingCart : Controller
    {
        private readonly RockyDbContext _db;
        private readonly HttpContext _httpContext;
        public ShoppingCart(RockyDbContext db, IHttpContextAccessor httpContext)
        {
            _db = db;
            _httpContext = httpContext.HttpContext;
        }
        public IActionResult Index()
        {
            List<CartItem> items = new List<CartItem>();
            List<CartItem> cartSession = _httpContext.Session.Get<List<CartItem>>(WC.SessionCart);
            if(cartSession != null && cartSession.Count()>0)
            {
                items = cartSession;
            }
            List<int> productIds = items.Select(i => i.ProductId).ToList();
            
            IEnumerable<Product> products = _db.Product.Where(p => productIds.Contains(p.Id));
            return View(products);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
           
            return RedirectToAction(nameof(Summary));
        }
        public IActionResult Summary()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);// object exist aft user login
            //var userId = User.FindFirstValue(ClaimTypes.Name);
            List<CartItem> items = new List<CartItem>();
            List<CartItem> cartSession = _httpContext.Session.Get<List<CartItem>>(WC.SessionCart);
            if (cartSession != null && cartSession.Count() > 0)
            {
                items = cartSession;
            }
            List<int> productIds = items.Select(i => i.ProductId).ToList();

            List<Product> products = _db.Product.Where(p => productIds.Contains(p.Id)).ToList();
            SummaryVM summaryVM = new SummaryVM()
            {
                ProductList = products,
                User = _db.AppUser.SingleOrDefault(u => u.Id == claim.Value)
            };
            return View(summaryVM);
        }
        public IActionResult Remove(int id)
        {
            List<CartItem> items = new List<CartItem>();
            List<CartItem> cartSession = _httpContext.Session.Get<List<CartItem>>(WC.SessionCart);
            if (cartSession != null && cartSession.Count() > 0)
            {
                items = cartSession;
            }
            items.Remove(items.SingleOrDefault(i => i.ProductId == id));
            _httpContext.Session.Set<List<CartItem>>(WC.SessionCart, items);

            return RedirectToAction(nameof(Index));
        }
    }
}
