using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using WEBLAPTOP.ViewModel;

namespace WEBLAPTOP.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private readonly DARKTHESTORE db = new DARKTHESTORE();
        public ActionResult Index()
        {

            int? id_kh = Session["id"] as int?;
            if (id_kh == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var gioHang = db.GIOHANGs.FirstOrDefault(g => g.ID_KH == id_kh);

            var cart = new List<WEBLAPTOP.ViewModel.CartItem>();

            if (gioHang != null)
            {
                var cartItemsQuery = from gh_sp in db.GIOHANG_SANPHAM
                                     join sp in db.SANPHAMs on gh_sp.ID_SP equals sp.ID_SP
                                     where gh_sp.ID_GH == gioHang.ID_GH
                                     select new WEBLAPTOP.ViewModel.CartItem
                                     {
                                         ID_SP = sp.ID_SP,
                                         TenSP = sp.TenSP,
                                         Images_url = sp.Images_url,
                                         GiaBan = sp.GiaBan,
                                         SoLuong = gh_sp.SoLuong,
                                         TongTien = gh_sp.SoLuong * sp.GiaBan
                                     };

                cart = cartItemsQuery.ToList();
            }
            ViewBag.TotalQuantity = cart.Sum(item => item.SoLuong.GetValueOrDefault(0));
            ViewBag.TotalPrice = cart.Sum(item => item.TongTien.GetValueOrDefault(0));
            return View(cart);
        }
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            int? id_kh = Session["id"] as int?;
            if (id_kh == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int id_kh_value = id_kh.Value;
            GIOHANG gioHang = db.GIOHANGs.FirstOrDefault(g => g.ID_KH == id_kh_value);
            if (gioHang == null)
            {
                gioHang = new GIOHANG { ID_KH = id_kh_value };
                db.GIOHANGs.Add(gioHang);
                db.SaveChanges();
            }
            var itemInCart = db.GIOHANG_SANPHAM.FirstOrDefault(
                item => item.ID_GH == gioHang.ID_GH && item.ID_SP == id
            );

            if (itemInCart != null)
            {
                itemInCart.SoLuong = (itemInCart.SoLuong ?? 0) + quantity;
            }
            else
            {
                var newItem = new GIOHANG_SANPHAM
                {
                    ID_GH = gioHang.ID_GH,
                    ID_SP = id,
                    SoLuong = quantity
                };
                db.GIOHANG_SANPHAM.Add(newItem);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateCart(int id_sp, int so_luong)
        {
            int? id_kh = Session["id"] as int?;
            if (id_kh == null) return RedirectToAction("Index", "Login");

            var gioHang = db.GIOHANGs.FirstOrDefault(g => g.ID_KH == id_kh);
            if (gioHang == null) return RedirectToAction("Index");

            var itemInCart = db.GIOHANG_SANPHAM.FirstOrDefault(
                item => item.ID_GH == gioHang.ID_GH && item.ID_SP == id_sp
            );

            if (itemInCart != null)
            {
                if (so_luong > 0)
                {
                    itemInCart.SoLuong = so_luong;
                }
                else
                {
                    db.GIOHANG_SANPHAM.Remove(itemInCart);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult RemoveFromCart(int id)
        {
            int? id_kh = Session["id"] as int?;
            if (id_kh == null) return RedirectToAction("Index", "Login");

            var gioHang = db.GIOHANGs.FirstOrDefault(g => g.ID_KH == id_kh);
            if (gioHang == null) return RedirectToAction("Index");

            var itemToRemove = db.GIOHANG_SANPHAM.FirstOrDefault(
                item => item.ID_GH == gioHang.ID_GH && item.ID_SP == id
            );

            if (itemToRemove != null)
            {
                db.GIOHANG_SANPHAM.Remove(itemToRemove);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
