using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit.Fixture.Mvc.Example.Models;

namespace Xunit.Fixture.Mvc.Example.Controllers
{
    [Route("date")]
    public class DateController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public DateDto Get() => new DateDto
        {
            UtcNow = DateTime.UtcNow
        };
    
        [HttpGet("future")]
        public DateDto GetFuture() => new DateDto
        {
            UtcNow = DateTime.UtcNow.Date.Add(_configuration.GetValue<TimeSpan>("future_offset"))
        };
    }
}
