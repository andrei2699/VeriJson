# VeriJson: Fluent JSON Assertions for .NET

**VeriJson** is a .NET library designed to make asserting JSON objects in your tests simple, intuitive, and highly
readable. Using a fluent API, VeriJson leverages `System.Text.Json` to provide powerful and flexible assertions for your
.NET applications.

## Why VeriJson?

Testing JSON responses or documents can often lead to verbose and hard-to-maintain assertion code. VeriJson aims to
solve this by:

* **Fluent Interface:** Chainable methods that make your assertions read like natural language.
* **Clear Error Messaging:** Get detailed error messages when assertions fail, pointing you directly to the discrepancy.
* **Configurability:** Customize assertion behaviors like case sensitivity, handling of extra properties, and numeric
  precision.
* **Lightweight and Focused:** Does one thing and does it well – JSON assertions.

## Installation

VeriJson is available on NuGet. You can install it using the .NET CLI:

```bash
dotnet add package VeriJson
```

Or via the NuGet Package Manager:

```bash
Install-Package VeriJson
```

## Getting Started

Once installed, you can start using VeriJson by adding the namespace and calling the Should() extension method on your
JSON string or JsonElement.

### Basic Usage

```csharp
using VeriJson;
using System.Text.Json;

public class MyTests
{
    
    [Fact]
    public void SimpleJsonObject_Should_PassAssertions()
    {
        string json = @"{""name"":""Widget"", ""id"":123, ""active"":true}";
        string json2 = @"{""name"":""Widget"", ""active"":true, ""id"":123}";

        json.Should().BeEquivalentTo(json2)
    }
    
    [Fact]
    public void SimpleJsonObject_Should_PassAssertions()
    {
        string json = @"{""name"":""Widget"", ""id"":123, ""active"":true}";

        json.Should().BeJsonObject()
            .And.HaveProperty("name").EqualTo("Widget")
            .And.HaveProperty("id").EqualTo(123)
            .And.HaveProperty("active").AsBoolean(b => b.BeTrue());
    }

    [Fact]
    public void SimpleJsonArray_Should_PassAssertions()
    {
        string json = @"[""apple"", ""banana""]";
        JsonElement jsonElement = JsonDocument.Parse(json).RootElement;

        jsonElement.Should().BeJsonArray()
            .And.HaveCount(2)
            .And.Contain("apple");
    }
}
```
