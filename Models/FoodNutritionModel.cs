using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyMe.Models
{
    public class FoodNutritionModel
    {
      
        public int FdcId { get; set; }

        public string FoodName { get; set; }
        public decimal? Calories { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Fat { get; set; }
        public decimal? Carbs { get; set; }


        // Add a dictionary to store additional nutrients dynamically
        public Dictionary<string, decimal?> AdditionalNutrients { get; set; } = new Dictionary<string, decimal?>();


        // New properties
        public string BrandName { get; set; }
        public string BrandedFoodCategory { get; set; }
        public decimal? ServingSize { get; set; }
        public string ServingSizeUnit { get; set; }
        public string Ingredients { get; set; }

    }
}
