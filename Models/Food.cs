using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyMe.Models
{
    public class Food
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FdcId { get; set; }

        public string FoodName { get; set; }
        public decimal? Calories { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Fat { get; set; }
        public decimal? Carbs { get; set; }


        // Navigation property
        public ICollection<MealItem> MealItems { get; set; }
    }
}
