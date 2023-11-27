using HealthyMe.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyMe.Models
{
    public class MealItem
    {
        public int MealItemId { get; set; }

        // Foreign key
        [ForeignKey("Meal")]
        public int MealId { get; set; }

        // Navigation property
        public Meal Meal { get; set; }


        // Foreign key
        [ForeignKey("Food")]
        public int FdcId { get; set; }

        // Navigation property
        public Food Food { get; set; }

        public decimal Quantity { get; set; }
    }
}
