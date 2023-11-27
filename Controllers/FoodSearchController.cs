using HealthyMe.Areas.Identity.Data;
using HealthyMe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using System.Net.Http;

namespace HealthyMe.Controllers
{
    public class FoodSearchController : Controller
    {

        private readonly HttpClient _httpClient;
        private List<Food> _searchResultsList;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<FoodSearchController> _logger;
        private readonly IHealthyMeRepository _repository;


        public FoodSearchController(IHttpClientFactory httpClientFactory,
            IHealthyMeRepository repository,
            UserManager<User> userManager,
            ILogger<FoodSearchController> logger)
        {
        
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://api.nal.usda.gov/fdc/v1/");
            _httpClient.DefaultRequestHeaders.Add("Api-Key", "MumFCNVpvxfc8MGVIFXlcMHIKZw0BhNMzYBU7dBy");
            _searchResultsList = new List<Food>();


            _repository = repository;
            _userManager = userManager;
            _logger = logger;

        }
        public IActionResult Index()
        {
            return View(_searchResultsList);
        }

        public async Task<IActionResult> FoodSearch(string query)
        {
            try
            {
             

                var apiKey = "MumFCNVpvxfc8MGVIFXlcMHIKZw0BhNMzYBU7dBy";

                var response = await _httpClient.GetAsync($"foods/search?api_key={apiKey}&query={query}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var searchResults = JsonConvert.DeserializeObject<dynamic>(json);
                var searchResultsList = new List<Food>();

             

                foreach (var food in searchResults.foods)
                {
                    var result = new Food
                    {
                        FoodName = food.description,
                        FdcId = food.fdcId
                     
                    };

                    searchResultsList.Add(result);
                }

                _searchResultsList = searchResultsList;

                return View("Index", _searchResultsList);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        public async Task<IActionResult> selectedFood(int query, string mealType, DateTime date, int quantity)
        {
            try
            {
               
               
                var apiKey = "MumFCNVpvxfc8MGVIFXlcMHIKZw0BhNMzYBU7dBy";

                var response = await _httpClient.GetAsync($"food/{query}?api_key={apiKey}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var selectedFood = JsonConvert.DeserializeObject<dynamic>(json);
                var userId = _userManager.GetUserId(User);



                    var food = new Food
                    {
                        FoodName = selectedFood.description,
                        FdcId = query,
                        Calories = GetNutrientValue(selectedFood.foodNutrients, "Energy"),
                        Protein = GetNutrientValue(selectedFood.foodNutrients, "Protein"),
                        Fat = GetNutrientValue(selectedFood.foodNutrients, "Total lipid (fat)"),
                        Carbs = GetNutrientValue(selectedFood.foodNutrients, "Carbohydrate, by difference"),

                    };

                // Save the food to the database
                _repository.CreateFood(food);


                var meal = new Meal
                {
                    mealType = mealType,
                    date = date,
                    UserId = userId
                };

                var mealItem = new MealItem
                {
                    FdcId = food.FdcId,
                    Quantity = quantity
                };

                // Save the meal and meal item to the database
                _repository.CreateFoodMealAndMealItem(food, meal, mealItem);

                return RedirectToAction("Index", "CalorieTracker");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error adding food to database: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Helper method to extract nutrient value by name
        private decimal? GetNutrientValue(dynamic foodNutrients, string nutrientName)
        {
            foreach (var nutrient in foodNutrients)
            {
                // Access the nutrient property to get the nested nutrientName
                var nutrientNameFromApi = nutrient.nutrient.name;

                if (nutrientNameFromApi == nutrientName)
                {
                    return nutrient.amount;
                }
            }

            return null; // Nutrient not found
        }

        public IActionResult EditFood(int id)
        {
            // Retrieve the food item from the database using the ID
            var existingFood = _repository.GetFoodById(id);

            if (existingFood == null)
            {
                // Handle the case where the food item is not found
                return NotFound();
            }
            _repository.DeleteFood(id);
            // Set ViewBag properties for the view
            ViewBag.ExistingFoodId = id;
            ViewBag.EditMode = true;

            return View("Index", _searchResultsList); // or return the appropriate view
        }

    }

}
