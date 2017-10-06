using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ParentsBank.Models;

namespace ParentsBank.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        public ActionResult Index()
        {
            foreach (Account a in db.Accounts) {
                if (a.Recipient == User.Identity.Name) {
                    return RedirectToAction("details", new { id =a.Id});
                }
            }
          
                
            
            return View(db.Accounts.Where(t => t.Owner == User.Identity.Name).ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Owner,Recipient,Name,OpenDate,InterestRate,Balance")] Account account)
        {
            if (ModelState.IsValid)
            {
                //int count = 0;
                if (db.Accounts.Where(u => u.Recipient == account.Recipient).ToList().Count > 0)
                {
                    ViewBag.doubleRecipientsError = "Recipient is already present in the system. Please enter another Recipient.";
                    return View(account);
                }
                if (db.Accounts.Where(u => u.Owner == account.Recipient).ToList().Count > 0)
                {
                    ViewBag.RecipientIsAlreadyOwnerError = "Recipient is already another owner in the system. Please enter another Recipient. ";
                }


                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            if (db.Accounts.Where(t => t.Owner == (User.Identity.Name)).ToList().Count > 0)
            {

                return View(account);
            }
            else {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Owner,Recipient,Name,OpenDate,InterestRate,Balance")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (db.Accounts.Where(t => t.Owner == (User.Identity.Name)).ToList().Count > 0)
            {
                return View(account);
            }
            else {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            if (db.Accounts.Where(t => t.Owner == (User.Identity.Name)).ToList().Count > 0)
            {
                return View(account);
            }
            else {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {


            Account account = db.Accounts.Find(id);
            if (account.Balance == 0)
            {
                db.Accounts.Remove(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
