using HealthyMe.Areas.Identity.Data;
using HealthyMe.Models;
using HealthyMe.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace HealthyMe.Controllers
{
    [Authorize]
    public class CalorieTrackerController : Controller
    {

        private readonly IHealthyMeRepository _repository;

        public CalorieTrackerController(IHealthyMeRepository repository)
        {
            _repository = repository;
        }
        public IActionResult Index(DateTime? date)
        {
            // Get all meals for the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            

            // If date is not provided, default to today
            if (!date.HasValue)
            {
                date = DateTime.Today;
            }

            // Get all meals for the current user and specified date
            var meals = _repository.GetMealsByUserIdAndDate(userId, date.Value);

            // Create the ViewModel
            var viewModel = new List<MealViewModel>();

            foreach (var meal in meals)
            {
                // Get associated foods for each meal
                var foods = _repository.GetFoodsForMeal(meal.mealId);
                var MealItems = _repository.GetMealItems();
                // Create a ViewModel for each meal
                var mealViewModel = new MealViewModel
                {
                    Meal = meal,
                    Foods = foods,
                    MealItems = MealItems
                };

                viewModel.Add(mealViewModel);
            }

            return View(viewModel);
        }

        
        public IActionResult Delete(int id)
        {
            // Call the repository method to delete the food item
            _repository.DeleteFood(id);

            // Redirect to the index page or another appropriate page
            return RedirectToAction(nameof(Index));
        }
    }
}
