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
    public class WishListItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WishListItems
        public ActionResult Index()
        {
            var wishListItems = db.WishListItems.Include(w => w.Account);
            return View(db.WishListItems.Where(t => t.Account.Owner == User.Identity.Name || t.Account.Recipient == User.Identity.Name).ToList());
        }

        // GET: WishListItems/Details/5
       
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishListItem wishListItem = db.WishListItems.Find(id);
            if (wishListItem == null)
            {
                return HttpNotFound();
            }
            return View(wishListItem);
        }

        // GET: WishListItems/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts.Where(t => t.Owner == User.Identity.Name), "Id", "Name");
            return View();
        }

        // POST: WishListItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishListItem wishListItem)
        {
            if (ModelState.IsValid)
            {
                db.WishListItems.Add(wishListItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts.Where(t => t.Owner == User.Identity.Name), "Id", "Name", wishListItem.AccountId);
            return View(wishListItem);
        }

        // GET: WishListItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishListItem wishListItem = db.WishListItems.Find(id);
            if (wishListItem == null)
            {
                return HttpNotFound();
            }
            if (db.Accounts.Where(t => t.Owner == (User.Identity.Name) || t.Recipient == (User.Identity.Name)).ToList().Count > 0)
            {
                ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishListItem.AccountId);
                return View(wishListItem);
            }
            else {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: WishListItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishListItem wishListItem)
        {
            if (ModelState.IsValid )
            {
                db.Entry(wishListItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishListItem.AccountId);
            return View(wishListItem);
        }
[HttpGet]
        public ActionResult Search(String listname)
        {
            bool SearchPerformed = false;
            var wishList_Items = db.WishListItems.Where(w => w.Account.Owner == User.Identity.Name);
            if (wishList_Items == null || wishList_Items.Count() == 0)
            {
                wishList_Items = db.WishListItems.Where(w => w.Account.Recipient == User.Identity.Name);
            }


            if (!string.IsNullOrWhiteSpace(listname))
            {
                wishList_Items = wishList_Items.Where(x => x.Description.Contains(listname));
                SearchPerformed = true;
                decimal n;
                bool isNumeric = Decimal.TryParse(listname, out n);
                if (isNumeric)
                {
                    var price = Convert.ToDecimal(listname);
                    if (wishList_Items == null || wishList_Items.Count() == 0)
                    {
                        wishList_Items = db.WishListItems.Where(w => w.Account.Recipient == User.Identity.Name);
                    }
                    wishList_Items = db.WishListItems.Where(w => w.Cost == price);
                    SearchPerformed = true;
                }
            }
            else
            {
                return View("Index", wishList_Items.ToList());
            }

            if (SearchPerformed)
            {
                // return search results
                return View("Index", wishList_Items.ToList());
            }

            // return empty list
            return View(new List<WishListItem>());
        }

        // GET: WishListItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishListItem wishListItem = db.WishListItems.Find(id);
            if (wishListItem == null)
            {
                return HttpNotFound();
            }
            if (db.Accounts.Where(t => t.Owner == (User.Identity.Name)).ToList().Count > 0)
            {
                return View(wishListItem);
            }
            else {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: WishListItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WishListItem wishListItem = db.WishListItems.Find(id);
            db.WishListItems.Remove(wishListItem);
            db.SaveChanges();
            return RedirectToAction("Index");
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
