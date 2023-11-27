// HealthyMeRepository.cs
using HealthyMe.Controllers;
using HealthyMe.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;


namespace HealthyMe.Areas.Identity.Data
{
    public class HealthyMeRepository : IHealthyMeRepository
    {
        private readonly HealthyMeContext _context;



        public HealthyMeRepository(HealthyMeContext context)
        {
            _context = context;
        } 

        // Implement other methods defined in the interface...

        public List<Meal> GetMealsByUserId(string userId)
        {
            return _context.Meals
                .Where(m => m.UserId == userId)
                .ToList();
        }

        public IEnumerable<Meal> GetMealsByUserIdAndDate(string userId, DateTime date)
        {
            return _context.Meals
                .Where(m => m.UserId == userId && m.date.Date == date.Date)
                .ToList();
        }

        public void CreateFood(Food food)
        {
            _context.Database.OpenConnection();
            try
            {
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Foods ON");

                _context.Foods.Add(food);
                _context.SaveChanges();
            }
            finally
            {
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Foods OFF");
                _context.Database.CloseConnection();
            }
        }





        public Meal GetMeal(string userId, string mealType, DateTime date)
        {
            return _context.Meals.Include(m => m.MealItems).FirstOrDefault(m => m.UserId == userId && m.mealType == mealType && m.date == date);
        }


        // Meals
        public void CreateMeal(Meal meal)
        {
            _context.Meals.Add(meal);
            _context.SaveChanges();
        }

        public List<Meal> GetMeals()
        {
            return _context.Meals.ToList();
        }

        public void UpdateMeal(Meal updatedMeal)
        {
            // Retrieve the existing meal from the database
            var existingMeal = _context.Meals
                .Include(m => m.MealItems)
                .FirstOrDefault(m => m.mealId == updatedMeal.mealId);

            if (existingMeal == null)
            {
                // Handle the case where the meal is not found
                return;
            }

            // Update the properties of the existing meal
            existingMeal.mealType = updatedMeal.mealType;
            existingMeal.date = updatedMeal.date;
            // Add more properties as needed

            // Update the meal items (assuming you want to update these too)
            foreach (var updatedMealItem in updatedMeal.MealItems)
            {
                var existingMealItem = existingMeal.MealItems.FirstOrDefault(mi => mi.MealItemId == updatedMealItem.MealItemId);

                if (existingMealItem != null)
                {
                    // Update properties of the existing meal item
                    existingMealItem.FdcId = updatedMealItem.FdcId;
                    existingMealItem.Quantity = updatedMealItem.Quantity;
                    // Add more properties as needed
                }
            }

            // Save changes to the database
            _context.SaveChanges();
        }

       

        public List<MealItem> GetMealItems()
        {
            return _context.MealItems.ToList();
        }
        public Meal GetMealById(int mealId)
        {
            return _context.Meals.Find(mealId);
        }

        public List<Food> GetFoodsForMeal(int mealId)
        {
            var mealItems = _context.MealItems
                .Include(mi => mi.Food)
                .Where(mi => mi.MealId == mealId)
                .ToList();

            return mealItems.Select(mi => mi.Food).ToList();
        }
      
        public Food GetFoodById(int fdcId)
        {
            return _context.Foods
                .FirstOrDefault(f => f.FdcId == fdcId);
        }
   

        public void DeleteFood(int fdcId)
        {

            // Find the food item to delete
            var food = _context.Foods
        .Include(f => f.MealItems)
            .ThenInclude(mi => mi.Meal)
        .FirstOrDefault(f => f.FdcId == fdcId);
            // Create a list to store the meals to be removed
            var mealsToRemove = new List<Meal>();
            if (food == null)
            {
                // Handle the case where the food item is not found
                return;
            }

            // Delete only the first associated meal and meal item
            // Specify the MealId you want to match for deletion
            var mealIdToDelete = food.MealItems.FirstOrDefault()?.Meal.mealId;

            // Delete only the associated meal and meal item with the specified MealId
            var mealItemToDelete = food.MealItems.FirstOrDefault();

            if (mealItemToDelete != null)
            {
                _context.MealItems.Remove(mealItemToDelete);
                
            }
            var mealToDelete = _context.Meals.Find(mealIdToDelete);
             _context.Meals.Remove(mealToDelete);
            
            // Save changes to the database to delete meal items and meals
            _context.SaveChanges();

            // Now, delete the food item
            _context.Foods.Remove(food);

            // Save changes to the database to delete the food item
            _context.SaveChanges();
        }

    

        public void UpdateMealItem(MealItem updatedMealItem)
        {
            _context.MealItems.Update(updatedMealItem);
            _context.SaveChanges();
        }



        public void CreateFoodMealAndMealItem(Food food, Meal meal, MealItem mealItem)
        {
            _context.Database.OpenConnection();
            try
            {
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.MealItems ON");

                // Assign the generated food ID to the meal item
                mealItem.MealItemId = food.FdcId;

            // Add meal to the context
            _context.Meals.Add(meal);
            _context.SaveChanges(); // Save changes to get the generated meal ID

            // Assign the generated meal ID to the meal item
            mealItem.MealId = meal.mealId;

            // Add meal item to the context
            _context.MealItems.Add(mealItem);
            _context.SaveChanges(); // Save changes to complete the transaction
            }
            finally
            {
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.MealItems OFF");
                _context.Database.CloseConnection();
            }
        }
    }
}
