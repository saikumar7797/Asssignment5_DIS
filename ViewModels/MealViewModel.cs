using HealthyMe.Models;

namespace HealthyMe.ViewModels
{
    public class MealViewModel
    {

        public Meal Meal { get; set; }
        public List<Food> Foods { get; set; }
        public List<MealItem> MealItems { get; set; }
    }
}
