[![Build status](https://ci.appveyor.com/api/projects/status/uma3136e7kwbcj94/branch/master?svg=true)](https://ci.appveyor.com/project/axle-h/xunit-fixture-mvc/branch/master)
[![NuGet](https://img.shields.io/nuget/v/xunit.fixture.mvc.svg)](https://www.nuget.org/packages/xunit.fixture.mvc)

# xunit-fixture-mvc

MVC functional tests with a fixture pattern.

For example:

```C#
[Fact]
public void When_creating_breakfast_item()
{
    using (var fixture = new MvcFunctionalTestFixture<Startup>(_output))
    {
        var request = new CreateOrUpdateBreakfastItemRequest { Name = "bacon", Rating = 10 };
        fixture.WhenCreating("BreakfastItem", request)
               .ShouldReturnSuccessfulStatus()
               .JsonResultShould<BreakfastItem>(r => r.Id.Should().Be(1),
                                                r => r.Name.Should().Be(request.Name),
                                                r => r.Rating.Should().Be(request.Rating));
    }
}
```