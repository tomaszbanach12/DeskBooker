using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeskBooker.Web.Pages
{
    public class BookDeskConfirmationModel : PageModel
    {
        public int DeskBookingId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Date { get; set; }

        public void OnGet(int deskBookingId, string firstName, string lastName, DateTime date)
        {
            DeskBookingId = deskBookingId;
            FirstName = firstName;
            LastName = lastName;
            Date = date;
        }
    }
}
