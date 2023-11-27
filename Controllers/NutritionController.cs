using HealthyMe.Areas.Identity.Data;
using HealthyMe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Newtonsoft.Json;

namespace HealthyMe.Controllers
{
    public class NutritionController : Controller
    {
        private readonly HttpClient _httpClient;
        private List<Food> _searchResultsList;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<NutritionController> _logger;
        private readonly IHealthyMeRepository _repository;
        private List<FoodNutritionModel> _foodNutritionModels;


        public NutritionController(IHttpClientFactory httpClientFactory,
            IHealthyMeRepository repository,
            UserManager<User> userManager,
            ILogger<NutritionController> logger)
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
        public async Task<IActionResult> NutritionFoodSearch(string query)
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

        public async Task<IActionResult> GetNutritionDetails(int query)
        {
            try
            {

                _searchResultsList.Clear();

                var apiKey = "MumFCNVpvxfc8MGVIFXlcMHIKZw0BhNMzYBU7dBy";

                var response = await _httpClient.GetAsync($"food/{query}?api_key={apiKey}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var selectedFood = JsonConvert.DeserializeObject<dynamic>(json);

                

                    var result = new FoodNutritionModel
                    {
                        FoodName = selectedFood.description,
                        FdcId = selectedFood.fdcId,
                        Calories = GetNutrientValue(selectedFood.foodNutrients, "Energy"),
                        Protein = GetNutrientValue(selectedFood.foodNutrients, "Protein"),
                        Fat = GetNutrientValue(selectedFood.foodNutrients, "Total lipid (fat)"),
                        Carbs = GetNutrientValue(selectedFood.foodNutrients, "Carbohydrate, by difference"),
                        BrandName = selectedFood.brandName,
                        BrandedFoodCategory = selectedFood.brandedFoodCategory,
                        ServingSize = selectedFood.servingSize,
                        ServingSizeUnit = selectedFood.servingSizeUnit,
                        Ingredients = selectedFood.ingredients
                    };

                // Add only relevant nutrient values to the dictionary
                var relevantNutrientNames = new List<string>
                    {
                        "Sugars, total including NLEA",
                        "Fiber, total dietary",
                        "Calcium, Ca",
                        "Iron, Fe",
                        "Sodium, Na","Vitamin C, total ascorbic acid",
                        // Add other relevant nutrient names
                    };

                foreach (var nutrient in selectedFood.foodNutrients)
                {
                    var nutrientNameFromApi = (string)nutrient.nutrient.name;
                    var nutrientAmount = nutrient.amount;

                    // Check if the nutrient is relevant
                    if (relevantNutrientNames.Contains(nutrientNameFromApi))
                    {
                        result.AdditionalNutrients[nutrientNameFromApi] = nutrientAmount;
                    }
                }
                var searchResultsList = new List<FoodNutritionModel> { result };

                _foodNutritionModels = searchResultsList;

                return View("FoodDetailsView", searchResultsList);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error getting nutrition details: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

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

    }
}
