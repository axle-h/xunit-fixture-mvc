[![Build status](https://ci.appveyor.com/api/projects/status/uma3136e7kwbcj94/branch/master?svg=true)](https://ci.appveyor.com/project/axle-h/xunit-fixture-mvc/branch/master)
[![NuGet](https://img.shields.io/nuget/v/xunit.fixture.mvc.svg)](https://www.nuget.org/packages/xunit.fixture.mvc)

# xunit-fixture-mvc

MVC functional tests with a fixture pattern.

## Example

```C#
public class BreakfastItemTests {
    private readonly ITestOutputHelper _output;

    public BreakfastItemTests(ITestOutputHelper output) {
        _output = output;
    }

    [Fact]
    public async Task When_creating_breakfast_item()
    {
        await new MvcFunctionalTestFixture<Startup>(_output)
                .WhenCreating("BreakfastItem", out CreateOrUpdateBreakfastItemRequest request)
                .ShouldReturnSuccessfulStatus()
                .JsonResultShould<BreakfastItem>(r => r.Id.Should().Be(1),
                                                 r => r.Name.Should().Be(request.Name),
                                                 r => r.Rating.Should().Be(request.Rating))
                .RunAsync();
    }
}
```

This already reads pretty well but let's break it down:

```C#
new MvcFunctionalTestFixture<Startup>(_output)
```

Configures the test server to use the specified startup class and to send all logs to the xunit test context.

```C#
.WhenCreating("BreakfastItem", out CreateOrUpdateBreakfastItemRequest request)
```

Configures the fixture to send a POST /BreakfastItem request to the test server once it's up and running. We're also asking the fixture to use AutoFixture to create a new instance of the request class and return it to us in an out parameter.

```C#
.ShouldReturnSuccessfulStatus()
```

Adds an assertion to the fixture that the response has a successful (2xx) code.

```C#
.JsonResultShould<BreakfastItem>(r => r.Id.Should().Be(1),
                                 r => r.Name.Should().Be(request.Name),
                                 r => r.Rating.Should().Be(request.Rating))
```

Adds an assertion to the fixture that the response body can be deserialized to an instance of BreakfastItem, that it's id property is 1 and that it's name and rating properties match those of the request.

```C#
.RunAsync();
```

This actually runs the fixture. The configuration will mean that it will:

1. Create a new test server to host the API that we're testing.
2. Send a POST /BreakfastItem request to the API with a JSON request body.
3. Assert that the response has a successful (2xx) code.
4. Attempt to deserialize the response body and run assertions on the deserialized object.
5. If no exceptions are thrown, do nothing and let the test pass. Otherwise:
   * If one exception is thrown => re-throw the exception.
   * If multiple exceptions are thrown => throw an aggregate exception containing all thrown exceptions.