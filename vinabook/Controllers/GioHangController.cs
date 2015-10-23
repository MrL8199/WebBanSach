﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vinabook.Models;

namespace Vinabook.Controllers
{
    public class GioHangController : Controller
    {

        QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        public List<CartItem> LayGioHang()
        {
            List<CartItem> lstGioHang = Session["ShoppingCart"] as List<CartItem>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<CartItem>();
                Session["ShoppingCart"] = lstGioHang;
            }
            return lstGioHang;
        }
        
        public ActionResult Index()
        {
            if (Session["ShoppingCart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<CartItem> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }
        //dem so luong san pham
        public ActionResult GioHangPartial()
        {
            int cartcount = 0;
            if (Session["ShoppingCart"] != null)
            {
                List<CartItem> ls = (List<CartItem>)Session["ShoppingCart"];
                foreach (CartItem item in ls)
                {
                    cartcount += item.Quality;
                }
            }
            ViewBag.count = cartcount;


            return PartialView();
        }
        [HttpPost]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NguoiDung");
            }
           
            return Json(new { Url = Url.Action("DatHangPartial") });
        }
        
        [HttpPost]
  
        public ActionResult DatHangPartial()
        {
            return PartialView("DatHangPartial");
        }
        [HttpPost]
        public ActionResult DatHangSubmit(string NgayGiao)
        {
            DonHang dh = new DonHang();
            if (Session["TaiKhoan"] != null || Session["TaiKhoan"].ToString() == "")
            {
                KhachHang customer = (KhachHang)Session["TaiKhoan"];
                if (ModelState.IsValid)
                {
                    dh.MaKH = customer.MaKH;
                    dh.NgayDat = DateTime.Now;
                    if(NgayGiao.Trim()!="")
                        dh.NgayGiao = Convert.ToDateTime(NgayGiao);
                    
                    dh.TinhTrangGiaoHang = 0;
                    dh.DaThanhToan = "Chưa thanh toán";
                    db.DonHangs.Add(dh);
                    db.SaveChanges();
                }
            }
            if (Session["ShoppingCart"] != null)
            {
                List<CartItem> ls = (List<CartItem>)Session["ShoppingCart"];
                foreach (CartItem item in ls)
                {
                    ChiTietDonHang CTDH = new ChiTietDonHang();
                    CTDH.MaDonHang = dh.MaDonHang;
                    CTDH.MaSach = item.productOrder.MaSach;
                    CTDH.SoLuong = item.Quality;
                    CTDH.DonGia = item.productOrder.GiaBan;
                    db.ChiTietDonHangs.Add(CTDH);
                    db.SaveChanges();
                }
            }
            Session["ShoppingCart"] = null;
            return Json(new { success = "Đặt hàng thành công!!!" });
        }
        [HttpPost]
        public ActionResult BackToCart()
        {
            return Json(new { Url = Url.Action("Success") });
        }
        //cap nhat gio hang
        [HttpPost]
        public ActionResult CapNhat(int id, int sl)
        {
            List<CartItem> listCartItem = (List<CartItem>)Session["ShoppingCart"];
            //nếu người dùng thêm hàng vào giỏ và lại trở về trang chủ thêm hàng tiếp 
            //thì session shoppingcart này có đang giữ tất cả sách trong giỏ hàng hiện tại hay không ?
            //có. cập nhật vào Session["ShoppingCart"] mà

            foreach (var item in listCartItem)
            {
                if (item.productOrder.MaSach == id)
                {
                    item.Quality = sl;
                    break;
                }
            }
            Session["ShoppingCart"] = listCartItem;
            return Json(new { Url = Url.Action("Success") });
        }
        [HttpPost]
        public ActionResult Success()
        {
            List<CartItem> lstGioHang = LayGioHang();
            return PartialView(lstGioHang);
        }
        //xoa gio hang
        [HttpPost]
        public ActionResult Remove(int id)
        {
            List<CartItem> listCartItem=(List<CartItem>)Session["ShoppingCart"];
            foreach( var item in listCartItem)
            {
                if (item.productOrder.MaSach == id)
                {
                    listCartItem.Remove(item);
                    break;
                }
            }
            Session["ShoppingCart"] = listCartItem;
            return Json(new { Url = Url.Action("Success") });
        }
    }
}