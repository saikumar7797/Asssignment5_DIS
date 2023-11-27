using HealthyMe.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyMe.Models
{
    public class Meal
    {
        public int mealId { get; set; }

        public string mealType { get; set; }

        public DateTime date { get; set; }

        // Foreign key
        [ForeignKey("id")]
        public string UserId { get; set; }

        // Navigation property
        public User id { get; set; }

        // Collection of MealItems
        public ICollection<MealItem> MealItems { get; set; }

    }
}
