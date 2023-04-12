using Microsoft.AspNetCore.Mvc.Rendering;

namespace HallHaven.Models
{
    public class FormViewModel
    {
        public SelectList Dorms { get; set; }
        public SelectList CreditHours { get; set; }
        public SelectList Majors { get; set; }

        public int SelectedDormId { get; set; }
        public int SelectedCreditHourId { get; set; }
        public int SelectedMajorId { get; set; }
    }
    
}
