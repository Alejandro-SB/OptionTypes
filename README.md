[![NuGet](https://img.shields.io/nuget/v/OptionTypes?label=OptionTypes)](https://www.nuget.org/packages/OptionTypes/)
[![NuGet](https://img.shields.io/nuget/v/OptionTypes.Ef?label=OptionTypes.Ef)](https://www.nuget.org/packages/OptionTypes.Ef/)

# OptionTypes

## Contents
- [Description](#description)
- [Usage](#usage)
    - [Unit](#unit)
    - [Option](#option)
    - [Result](#result)
- [Design Philosophy](#design-philosophy)
- [What's missing](#whats-missing)


## Description
_OptionTypes_ is a package to use some useful monads in C#. It contains 2 classes:

- The `Option<T>` class allows to create an item of type T that may have no value. This value cannot be accessed in an unsafe manner by design, making really easy to completely remove null references from your code and reducing the number of `NullReferenceException` exceptions thrown.

- The `Result` class represents an operation that has been completed. This reduces the number of `try/catch` blocks needed to manage application flow, making error management explicit and ending with more reliable code.

## Usage

### Unit
Unit is a helper type to represent the absence of value (think of it as void). Because in functional programming every function returns a value, it is added here for compatibility.

### Option

Create a new Option using one of the helper methods (`Option.FromValue`, `Option.Some`, and `Option.None`):
```Csharp
var optionInt = Option.Some(1);
var optionFloat = Option.FromValue(12);
string? nullableString = null;
var optionString = Option.FromValue(nullableString);
```

Map its content using the `Map` method:
```Csharp
var optionText = await ReadTextFromFile(filePath);

var uppercaseText = optionText.Map(text => text.ToUpper());
```

You can also map to another `Option` and it will be flatten:
```Csharp
var number = Option.FromValue(1);

// double type is Option<int>
var double = number.Map(x => Option.FromValue(x * 2));
```

If you want to check both options, use the `Match` method:
```Csharp
let user = await GetUser();


var userName = user.Match(x => x.Name, () => "User not found");
```

In case you want to provide a fallback value, you can use `ValueOr`:
```Csharp
var optionUserName = await GetUserName();
var userName = optionUserName.ValueOr("Unknown user");
```

In case you want to do something if there is a value present, you can use the `IsSome` method:
```Csharp
Option<User> optionUser = await GetUser();

if(!optionUser.IsSome(out User user))
{
    return Results.NotFound();
}

var posts = postService.GetPostsByUserId(user.Id);
```

You can force the value out using the `Unwrap` method. This approach is **not recommended**:
```Csharp
var optionValue = Option.Some(1);

var value = optionValue.Unwrap() // 1

var optionString = Option<string>.None();
optionString.Unwrap(); // throws NullReferenceException
```

There are also extension methods for `Task<Option<T>>` so you can chain `Map`, `Match`, `ValueOr`, and `Unwrap` to your tasks.
```Csharp
var userBalance = GetUser() // GetUser returns a Task<Option<User>>
                    .Map(user => bankService.GetAccounts(user.Id))
                    .Map(accounts => accounts.Sum(a => a.Balance))
                    .ValueOr(0m);
```

### Result

You can create an instance using the Ok/Err static methods:
```Csharp
var okResult = Result<Unit, ProcessError>.Ok(default);
var errorResult = Result<Unit, ProcessError>.Err(ProcessError.DatabaseConnection);
```

You can map the ok value or err value using `Map` and `MapErr` methods:
```Csharp
var okResult = Result<int, Exception>.Ok(3).Map(x => x*2)); // Ok(6)
var errResult = Result<Unit, string>.Err("failure").MapErr(x => x.ToUpper()); // Err("FAILURE")
```

To provide handlers for both cases, which should be the normal usage, use the `Match` method:
```Csharp
var result = await CreateUser();

result.Match(
    user => Results.Created("/user", { id: user.Id }),
    err => err switch {
        CreateUserError.EmailExists => Results.Conflict(),
        _ => Results.BadRequest()
    });
```

Sometimes you only want to know if an operation has completed successfully to get the ok value. You can use the `Ok` method:
```Csharp
var parsingResult = ParseLines(path);

var linesParsed = parsingResult.Ok().ValueOr(0);

Console.WriteLine($"Parsed {linesParsed} lines");
```

In order to get early returns when needed, there is an `IsErr` method:
```Csharp
Result<User, string> userCreationResult = await CreateUser(userPayload);

if (userCreationResult.IsErr(out User user, out string error))
{
    return Result<Unit, UserCreationError>.Err(UserCreationError.CannotCreateUser);
}

var userRoleResult = await AssignRoles(user, Roles.Admin);

if (userRoleResult.IsErr(out var roleError)) 
{
    return Result<Unit, UserCreationError>.Err(UserCreationError.CannotAssignRole);
}

emailService.NotifyUser(user.Email);
```

As with `Option`, there are some extensions in Task to be able to chain methods:
```Csharp
public async IResult Post([FromBody] UserPayload payload)
    => await CreateUser(payload).Match<IResult>(
        user => Results.Created("/user", { id: user.Id }),
        err => err switch {
            CreateUserError.EmailExists => Results.Conflict(),
            _ => Results.BadRequest()
        });
```

## Design philosophy
The idea behind this small package was to provide `Option`/`Option`/`Result` monads that work idiomatically with C#, whithout losing the essence of them.

In order to achieve this, an approach of *Explicit better than implicit* was used:
- When working with `Option`, minimize the posibility of `NullReferenceException` by limiting the options to get the value out, enforcing the developer to handle all the cases.
- When working with `Result`, minimize the risk of unforseen consequences (λ) by encouraging to use the `Match` statement.
- Encourage the usage of Error values, let it be records with some payload or enums, that provide useful information and force the developer to take action for each one of them. By being explicit in what kind of errors can pop out, the developer is forced to handle all the cases than can go wrong and not rely on catch blocks.

## What's missing
Because of the limitations of C#, some things cannot be achieved. Here's a small list:
- In order to create a `Result`, both types should be specified. This is annoying, because when you are using large class names, you end up doing:
    ```Csharp
    public async Task<Result<CreateUserReturnValue, CreateUserError>> CreateUser(CreateUserPayload payload)
    {
    ...
        if (**somecondition**)
        {
            return Result<CreateUserReturnValue, CreateUserError>.Err(...);
        }
    ...
    }
    ```
    This can be mitigated by using `using` alias like this:
    ```Csharp
    using CreateUserResult = OptionTypes.Result<UseCases.CreateUserReturnValue, UseCases.CreateUserError>;

    public async Task<CreateUserResult> CreateUser(CreateUserPayload payload)
    {
    ...
        if (**somecondition**)
        {
            return CreateUserResult.Err(...);
        }
    ...
    }
    ```
- `Result<Unit, _>` feels weird, as you have to manually do `Result<Unit, _>.Ok(default)` or `Result<Unit, _>.Ok(new Unit())`. No workaround for this I'm afraid.
- Early returns feel off. I would've loved to have something similar to [Rust's question mark](https://doc.rust-lang.org/rust-by-example/std/result/question_mark.html), but `Option.IsSome` and `Result.IsErr` are the closest things I could think of.
- The absence of `union types`/`discriminated unions`/`closed enums` make managing the different options underwhelming and unreliable if you are not careful. If you have a method like this:
```Csharp
enum ProcessError
{
    FailureA,
    FailureB
}

Result<Unit, ProcessError> DoProcess() { ... }

public void Run() 
{
    var result = DoProcess();

    result.Match(_ => Console.WriteLine("Success"),
    err => err switch
    {
        ProcessError.FailureA => Console.WriteLine("FailureA"),
        ProcessError.FailureB => Console.WriteLine("FailureB"),
    });
}
```
You may think that you are handling everything, as the enum only has 2 options, but you would receive a warning. 
This is because enums are `int` in C#, so you could do `(ProcessError)435627` and pass. 
Their proposed solution is to add a general case `_ => WHATEVER` but this is exactly what this library is trying to avoid. Again, the goal is to be explicit, because if you add a default case, you will not receive a warning nor an error when you add another error in `ProcessError`. So the only option for now is to disable the rule. Ending like this:
```Csharp
enum ProcessError
{
    FailureA,
    FailureB
}

Result<Unit, ProcessError> DoProcess() { ... }

public void Run() 
{
    var result = DoProcess();

    result.Match(_ => Console.WriteLine("Success"),
    #pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
    err => err switch
    {
        ProcessError.FailureA => Console.WriteLine("FailureA"),
        ProcessError.FailureB => Console.WriteLine("FailureB"),
    });
    #pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
}
```

## Usage inside Entity Framework Core
**This uses the internal EF Api**

The project OptionTypes.Ef contains the `ValueConverters` needed to map the `Option<T>` type to the Entity Framework columns. 

In the `OnModelCreating` method overriden in your DbContext, call `AddOptionTypeConverters`.
This will add all the converters needed in your model.