using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthyMe.Models;
using Microsoft.AspNetCore.Identity;

namespace HealthyMe.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    // Add a navigation property for meals
    public ICollection<Meal> Meals { get; set; }
}

