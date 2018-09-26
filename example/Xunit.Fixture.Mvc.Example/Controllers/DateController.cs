using System;
using Microsoft.AspNetCore.Mvc;
using Xunit.Fixture.Mvc.Example.Models;

namespace Xunit.Fixture.Mvc.Example.Controllers
{
    [Route("date")]
    public class DateController : Controller
    {
        [HttpGet]
        public DateDto Get() => new DateDto
                                {
                                    UtcNow = DateTimeOffset.UtcNow
                                };
    }
}
