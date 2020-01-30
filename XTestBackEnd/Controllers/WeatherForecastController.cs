using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XTestBackEnd.Interfaces;
using XTestBackEnd.Models;

namespace XTestBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRepository<User> _userRepo;
        public WeatherForecastController(
            IRepository<User> userRepo,
            ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _userRepo = userRepo; 
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            var user = new User() { FirstName = "1st", SecondName = "2st", Description="", EMail="", GoogleID="" };
            _userRepo.Create(user);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
            
        }
    }
}
