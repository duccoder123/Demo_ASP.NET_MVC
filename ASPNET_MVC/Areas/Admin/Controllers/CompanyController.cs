using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.DataAccess.Data;
using MVC.DataAccess.Repository.IRepository;
using MVC.Models;
using MVC.Models.ViewModel;
using MVC.Utility;
using System.Collections.Generic;

namespace ASPNET_MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompany = _unitOfWork.Company.GetAll().ToList();
           
            return View(objCompany);
        }
        public IActionResult Upsert(int? id) // -> Upsert = Update + Insert
        {
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
            //    .GetAll().Select(u => new SelectListItem
            //    {
            //        Text = u.Name,
            //        Value = u.Id.ToString()
            //    });
            //ViewBag.CategoryList = CategoryList;
           
            if(id == 0 || id == null)
            {
                //create
                return View(new Company());
            }
            else
            {
                // update
               Company Companies = _unitOfWork.Company.Get(u => u.Id == id);
                return View(Companies);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company companyObj, IFormFile? file)
        {
            //custom validations
            //if(obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "Display Order không được trùng với Name");
            //}
            if (ModelState.IsValid)
            {
               

                if (companyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(companyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(companyObj);
                }
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                // RedirectToAction để chuyển hướng trang khi thực hiện xong một tác vụ
                return RedirectToAction("Index");
            }
            else
            {

                
                return View(companyObj);
            }
           
        }

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? CompanyfromDb = _unitOfWork.Company.Get(u => u.Id == id);
        //    //Category category = _db.Categories.FirstOrDefault(c => c.Id == id);
        //    //Category category = _db.Categories.Where(c=> id == c.Id).FirstOrDefault();   
        //    if (CompanyfromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(CompanyfromDb);
        //}
        //[HttpPost]
        //public IActionResult Edit(Company obj)
        //{
        //    //custom validations
        //    //if(obj.Name == obj.DisplayOrder.ToString())
        //    //{
        //    //    ModelState.AddModelError("name", "Display Order không được trùng với Name");
        //    //}
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Company.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Company edited successfully";

        //        // RedirectToAction để chuyển hướng trang khi thực hiện xong một tác vụ
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? CompanyfromDb = _unitOfWork.Company.Get(u => u.Id == id);
        //    //Category category = _db.Categories.FirstOrDefault(c => c.Id == id);
        //    //Category category = _db.Categories.Where(c=> id == c.Id).FirstOrDefault();   
        //    if (CompanyfromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(CompanyfromDb);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    //custom validations
        //    //if(obj.Name == obj.DisplayOrder.ToString())
        //    //{
        //    //    ModelState.AddModelError("name", "Display Order không được trùng với Name");
        //    //}
        //    Company? obj = _unitOfWork.Company.Get(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Company.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Company deleted successfully";

        //    // RedirectToAction để chuyển hướng trang khi thực hiện xong một tác vụ
        //    return RedirectToAction("Index");
        //}
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

       
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if(CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();
            List<Company> objCompanyList = _unitOfWork.Company.GetAll(includeProperties: "Category").ToList();
            return Json(new { success = true, message = "Delete Successful"});
        }
        #endregion
    }
}
