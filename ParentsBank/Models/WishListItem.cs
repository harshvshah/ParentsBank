using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    public class WishListItem
    {
        public WishListItem() {
            DateAdded = DateTime.Now.Date;
        }

        public int Id { get; set; }

        public int AccountId { get; set; }

        public virtual Account Account { get; set; }

        public DateTime DateAdded { get; set; }

        [Required(ErrorMessage = "Cost is required!")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public String Description { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public String Link { get; set; }

        public Boolean Purchased { get; set; }


    }
}