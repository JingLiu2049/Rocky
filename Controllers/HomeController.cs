using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RockyDbContext _db;
        private readonly HttpContext _httpContext;

        public HomeController(ILogger<HomeController> logger, RockyDbContext db, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _db = db;
            _httpContext = httpContext.HttpContext;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _db.Product.Include(u => u.Category).Include(u => u.Application),
                Categories = _db.Category
            };

            return View(homeVM);
        }

        public async Task<IActionResult> Detail(int? id)
        {   
            if (id == null )
            {
                return NotFound();
            }
            Product product = await _db.Product
                .Include(u => u.Category)
                .Include(u => u.Application)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var cartItems= _httpContext.Session.Get < List<CartItem>>(WC.SessionCart);
            bool inCart = false;
            if (cartItems!=null)
            {
                CartItem result = cartItems.Find( i => i.ProductId == id);
                inCart = (result != null);
            }
            
            
            DetailVM detailVM = new DetailVM()
            {
                Product = product,
                IsInCart = inCart
            };

            return View(detailVM);
        }

        [HttpPost,ActionName("Detail")]
        public IActionResult DetailPost(DetailVM detailVM)
        {
            if (detailVM == null)
            {
                return NotFound();
            }

            List<CartItem> cartItems = new List<CartItem>();
            var sessionCart = _httpContext.Session.Get<List<CartItem>>(WC.SessionCart);
            if(sessionCart!=null && sessionCart.Count() > 0)
            {
                cartItems = sessionCart;
            }
            cartItems.Add(new CartItem() { ProductId = detailVM.Product.Id });
            _httpContext.Session.Set<List<CartItem>>(WC.SessionCart, cartItems);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveFromCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            List<CartItem> cartItems = new List<CartItem>();
            List<CartItem>  cartSession = _httpContext.Session.Get<List<CartItem>>(WC.SessionCart);
            if (cartSession != null && cartSession.Count() > 0)
            {
                cartItems = cartSession;
            }
            var itemToRemove = cartItems.SingleOrDefault(i => i.ProductId == id);
            if(itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
            }
            _httpContext.Session.Set(WC.SessionCart, cartItems);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
