using HealthyMe.Models;
using System.Collections.Generic;

namespace HealthyMe.Areas.Identity.Data
{
    public interface IHealthyMeRepository
    {
        // Meals
        void CreateMeal(Meal meal);
        List<Meal> GetMeals();
        void UpdateMeal(Meal updatedMeal);
       

        // Foods
        void CreateFood(Food food);
       
        void DeleteFood(int fdcId);
        Food GetFoodById(int fdcId);
       
        List<MealItem> GetMealItems();
        void UpdateMealItem(MealItem updatedMealItem);
     

        Meal GetMealById(int mealId);
        List<Food> GetFoodsForMeal(int mealId);


        // Create a food, a meal, and a meal item at the same time
        void CreateFoodMealAndMealItem(Food food, Meal meal, MealItem mealItem);
        // Retrieve meals by user ID
        List<Meal> GetMealsByUserId(string userId);
        Meal GetMeal(string userId, string mealType, DateTime date);

        // Retrieve meals by user ID and date
        IEnumerable<Meal> GetMealsByUserIdAndDate(string userId, DateTime date);

    }
}
