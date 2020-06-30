using jQueryAjaxInAsp.NETMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQueryAjaxInAsp.NETMVC.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewAll()
        {
            return View(GetAllContact());
        }

        IEnumerable<Contact> GetAllContact()
        {
            using (DBModel db = new DBModel())
            {
                return db.Contacts.ToList<Contact>();
            }

        }

        public ActionResult AddOrEdit(int id = 0)
        {
            Contact contact = new Contact();
            if (id != 0)
            {
                using (DBModel db = new DBModel())
                {
                    contact = db.Contacts.Where(x => x.ContactID == id).FirstOrDefault<Contact>();
                }
            }
            return View(contact);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Contact contact)
        {
            try
            {
                if (contact.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(contact.ImageUpload.FileName);
                    string extension = Path.GetExtension(contact.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    contact.ImagePath = "~/AppFiles/Images/" + fileName;
                    contact.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                using (DBModel db = new DBModel())
                {
                    if (contact.ContactID == 0)
                    {
                        db.Contacts.Add(contact);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(contact).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllContact()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    Contact contact = db.Contacts.Where(x => x.ContactID == id).FirstOrDefault<Contact>();
                    db.Contacts.Remove(contact);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllContact()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
