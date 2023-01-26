[![NuGet](https://img.shields.io/nuget/v/OptionTypes?label=OptionTypes)](https://www.nuget.org/packages/OptionTypes/)
[![NuGet](https://img.shields.io/nuget/v/OptionTypes.Ef?label=OptionTypes.Ef)](https://www.nuget.org/packages/OptionTypes.Ef/)

# OptionTypes
## Description
_OptionTypes_ is a package to use some useful monads in C#. It contains basically 2 classes:

- The `Maybe<T>` class allows to create an item of type T that may have no value. This value cannot be accessed in an unsafe manner, making really easy to completely remove null references from your code and reducing the number of `NullReferenceException` thrown.

- The `Result` class represents an operation that has been completed. It's slim API makes taking a decision based on the result of the operation straightforward.

## How to use it
To create a variable as a `Maybe<T>`, helper methods in the static `Maybe` class can be used, or use the static `Some`/`None` methods in `Maybe<T>` class.

To use the value inside the `Maybe` class, use the `Map`, `Bind`, `Match` or `ValueOr` methods.

To create a `Result` type, use the static methods `Ok`/`Error`.

## Usage example
### Maybe
```csharp
public static async Task Main(string[] args)
{
    var userTelephone = await GetUser(args[0])
        .Map(u => u.Telephone);

    Console.WriteLine(userTelephone.ValueOr("Telephone not found"));
}

private static async Task<Maybe<User>> GetUser(string id)
{
    var user = await UserManager.GetAsync(id);

    return Maybe.FromValue(user);
}
```

### Result
```csharp
public Result<Unit, string> WriteFile(string path, byte[] content)
{
    try
    {
        File.WriteAllBytes(path, content);

        return Result<Unit, string>.Ok(default);
    }
    catch
    {
        return Result<Unit, string>.Error("Error");
    }
}

public static void Main(string[] args)
{
    var result = WriteFile(path, content);

    result.Match(
        _ => Console.WriteLine("Success"),
        err => Console.WriteLine(err)
    );
}
```

## Usage inside Entity Framework Core
**This uses the internal EF Api**

The project OptionTypes.Ef contains the `ValueConverters` needed to map the `Maybe<T>` type to the Entity Framework columns. 

In the `OnModelCreating` method overriden in your DbContext, call `AddOptionTypeConverters`.
This will add all the converters needed in your model.