﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    [CustomValidation(typeof(Account), "checkDuplicateEmailAddress")]
    public class Account
    {

        public Account()
        {
            Transactions = new List<Transaction>();
            OpenDate = DateTime.Now;
        }

        //Generated by the database
        public int Id { get; set; }

        //Email address of the owner
        [Required(ErrorMessage = "Owner email Address is mandatory")]
        [EmailAddress]

        public string Owner { get; set; }

        //Email address of the child
        [Required]
        [EmailAddress(ErrorMessage = "Recipient email address is mandatory")]
        public string Recipient { get; set; }

        //name after the child
        [Required(ErrorMessage = "Account Name is mandatory!")]
        public string Name { get; set; }

        //date, the account was created
        [Editable(false)]
        public DateTime OpenDate { get; set; }

        //Interest percent earned on the account
        [Range(1, 99, ErrorMessage = "Range cannot be less than or equal to 0 or greater than or equal to 100")]
        public decimal InterestRate { get ; set; }

        public decimal Balance { get {
                decimal balance = 0;
                foreach (Transaction t in Transactions) {
                    balance = balance + t.Amount;
                }
                return balance;
            }
            set{; } }

        public virtual List<Transaction> Transactions { get; set; }

        public virtual List<WishListItem> WishListItems { get; set; }

        public static ValidationResult checkDuplicateEmailAddress(Account account, ValidationContext context)
        {
            if (account == null)
            {

                return ValidationResult.Success;
            }

            if (account.Owner == account.Recipient)
            {
                return new ValidationResult("Owner and Recipient cannot be same!!!");
            }

            return ValidationResult.Success;
        }

        public bool IsOwner(string currentUser)
        {
            // HELPER METHOD TO CHECK IF THE USER PASSED IN AS THE ARGUMENT
            // IS THE OWNER
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return false;
            }

            if (currentUser.ToUpper() == Owner.ToUpper())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsRecipient(string currentUser)
        {
            // HELPER METHOD TO CHECK IF THE USER PASSED IN AS THE ARGUMENT
            // IS THE ADMINISTRATOR
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return false;
            }

            if (currentUser.ToUpper() == Recipient.ToUpper())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsOwnerOrRecipient(string currentUser)
        {
            // HELPER METHOD TO CHECK IF THE USER PASSED IN AS THE ARGUMENT
            // IS THE OWNER OR THE ADMINISTRATOR
            return IsOwner(currentUser) || IsRecipient(currentUser);
        }

        public decimal CalculateEndofYearPrincipalandInterestAmount()
        {

            /* 
              DateTime StartDate = new DateTime(DateTime.Now.Year, 1, 1);
              DateTime EndDate =DateTime.Now;
              decimal interest = InterestRate;
              TimeSpan duration = EndDate - StartDate;
              int numberOfDaysinYear = duration.Days;
             // int compoundingTimes = 12;

              decimal InterestAmount = 0;
              decimal RunningTotal = 0m;
              decimal Principal = 0m;

              for(int i = 1 ; i<= numberOfDaysinYear; i++) {
                  DateTime currentDate = StartDate.AddDays(i-1);
                  foreach (Transaction t in Transactions) {
                      if (t.TransactionDate == currentDate) {
                          RunningTotal = RunningTotal + t.Amount;
                      }

                  }

                  }

              if (RunningTotal != 0)
              {
                  double timePeriod = 1 / numberOfDaysinYear;
                  //double Power = Math.Pow((double)(1 + (interest / 12)), (double)(12 * (i / numberOfDaysinYear)));

                  //decimal Amount = RunningTotal * (decimal)Power;
                  decimal Amount = RunningTotal * (decimal)Math.Pow((1 + (double)InterestRate / 12), 12 * (1 / (double)numberOfDaysinYear));
                  RunningTotal = Amount;

                  InterestAmount = RunningTotal - Balance;
              }

              return Math.Round(InterestAmount,2);
      */


            double CI = 0;
            if (Transactions != null && Transactions.Count > 0)
            {
                int year = DateTime.Now.Year;
                DateTime firstDay = new DateTime(year, 1, 1);
                DateTime lastDay = new DateTime(year + 1, 1, 1);
                double numberOfDays = (lastDay - firstDay).Days;
                double compundingTimes = 12;
                double runningTotal = 0;
                for (DateTime date = firstDay; date.Date <= DateTime.Today.Date; date = date.AddDays(1))
                {
                    var dailySum = Transactions.Where(b => b.TransactionDate == date).ToList().Sum(g => g.Amount);
                    runningTotal += (double)dailySum;
                    if (runningTotal > 0)
                    {
                        // (1 + r/n)
                        double body = 1 + ((double)InterestRate / 1200);
                        //n(d/366)
                        //float diff = (DateTime.Today - t.TransactionDate).Days;
                        double exponent = (compundingTimes / numberOfDays);
                        //double exponent = (12.00 * (diif));
                        // P(1 + r/n)^nt-P
                        runningTotal = (runningTotal * Math.Pow(body, exponent));
                    }

                }
                CI = runningTotal -(double) Balance;

            }
            return Math.Round((decimal)CI,2);





        }



    }

   


}
