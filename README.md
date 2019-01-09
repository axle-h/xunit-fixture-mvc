[![Build status](https://ci.appveyor.com/api/projects/status/uma3136e7kwbcj94/branch/master?svg=true)](https://ci.appveyor.com/project/axle-h/xunit-fixture-mvc/branch/master)
[![NuGet](https://img.shields.io/nuget/v/xunit.fixture.mvc.svg)](https://www.nuget.org/packages/xunit.fixture.mvc)

# xunit-fixture-mvc

MVC functional tests with a fixture pattern.

## Example

```C#
public class BreakfastItemTests {
    private readonly ITestOutputHelper _output;

    public BreakfastItemTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public Task When_creating_breakfast_item() =>
        new MvcFunctionalTestFixture<Startup>(_output)
            .WhenCreating("BreakfastItem", out CreateOrUpdateBreakfastItemRequest request)
            .ShouldReturnSuccessfulStatus()
            .ShouldReturnEquivalentJson<BreakfastItem, CreateOrUpdateBreakfastItemRequest>(request)
            .ShouldReturnJson<BreakfastItem>(r => r.Id.Should().NotBeNullOrEmpty())
            .RunAsync();
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

Configures the fixture to send a `POST /BreakfastItem` request to the test server once it's up and running. We're also asking the fixture to use [AutoFixture](https://github.com/AutoFixture/AutoFixture) to create a new instance of the request class and return it to us in an out parameter.

```C#
.ShouldReturnSuccessfulStatus()
```

Adds an assertion to the fixture that the response has a successful (2xx) code.

```C#
.ShouldReturnEquivalentJson<BreakfastItem, CreateOrUpdateBreakfastItemRequest>(request)
.ShouldReturnJson<BreakfastItem>(r => r.Id.Should().NotBeNullOrEmpty())
```

Both of these add an assertion to the fixture that the response body is JSON that can be deserialized to an instance of `BreakfastItem`. The first line adds an assertion that the deserialized response is equivalent to the request object according to [FluentAssertions](https://github.com/fluentassertions/fluentassertions) amazing equivalence feature. The second line adds an assertion that the `Id` property of the response has been set to something other than null or the empty string.

```C#
.RunAsync();
```

This actually runs the fixture. The configuration will mean that it will:

1. Create a new test server to host the API that we're testing.
2. Send a `POST /BreakfastItem` request to the API with a JSON request body serialized from the generated `CreateOrUpdateBreakfastItemRequest`.
3. Assert that the response has a successful (2xx) code.
4. Attempt to deserialize the response body as a `BreakfastItem` and run assertions on the deserialized object.
5. If no exceptions are thrown, do nothing and let the test pass. Otherwise:
   * If one exception is thrown => re-throw the exception.
   * If multiple exceptions are thrown => throw an aggregate exception containing all thrown exceptions.