using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DataAccess.Repository.IRepository;
using MVC.Models;
using MVC.Models.ViewModel;
using System.Security.Claims;

namespace ASPNET_MVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product")
            };
            foreach(var cart in  ShoppingCartVM.ShoppingCartList)
            {
                 cart.Price = GetPriceBaseOnQuanlity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);    
        }

        public IActionResult Plus(int cardId)
        {
            var cartfromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cardId);
            cartfromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartfromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));   
        }

        public IActionResult Minus(int cardId)
        {
            var cartfromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cardId);
            if (cartfromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartfromDb);
            }
            else
            {
                cartfromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartfromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Summary()
        {
            return View();
        }
        public IActionResult Remove(int cardId)
        {
            var cartfromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cardId);
            _unitOfWork.ShoppingCart.Remove(cartfromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBaseOnQuanlity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if(shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
