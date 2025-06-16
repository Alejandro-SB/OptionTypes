[![NuGet](https://img.shields.io/nuget/v/Funzo?label=Funzo)](https://www.nuget.org/packages/Funzo/)
[![NuGet](https://img.shields.io/nuget/v/Funzo.Serialization?label=Funzo.Serialization)](https://www.nuget.org/packages/Funzo.Serialization/)
[![NuGet](https://img.shields.io/nuget/v/Funzo.SourceGenerators?label=Funzo.SourceGenerators)](https://www.nuget.org/packages/Funzo.SourceGenerators/)

# Funzo

## Contents
- [Where is this coming from](#where-is-this-coming-from)
- [Description](#description)
- [Usage (recommended way)](#usage-recommended-way)
- [Documentation](#documentation)
    - [Unit](#unit)
    - [Option](#option)
    - [Result](#result)
    - [Union](#union)
- [Serialization](#serialization)
- [Example](#example)
- [Further work](#further-work)
- [Design Philosophy](#design-philosophy)
- [What's missing (updated after adding union types & source generators)](#whats-missing-updated)

## Where is this coming from
This package was previously called OptionTypes, but given that now more things are being added, I think a new name could fit better. Wanted to make it sound like something functional, so Funzo it is. Sounds fine.
Apart from the name, the `Maybe<T>` class was renamed to `Option<T>`. It is more common than `Maybe<T>` so I thought it would be better for people to identify it. I ~~copied a lot of things~~ took inspiration of the work done by [`OneOf`](https://github.com/mcintyre321/OneOf) (specially source generators) but decided to adapt it to my way of doing things: **making everything explicit**.

## Description
_Funzo_ allows the developer to use some classes more commonly used in functional programming for error-proof programming and better type definition. It contains 4 classes:

- The `Unit` class represents an empty class. Because functions always return something, `Unit` is the equivalent to `void`

- The `Option<T>` class allows to create an item of type T that may have no value. This value cannot be accessed in an unsafe manner by design, making really easy to completely remove null references from your code and reducing the number of `NullReferenceException` exceptions thrown.

- The `Result<TOk, TErr>` class represents an operation that has been completed. This reduces the number of `try/catch` blocks needed to manage application flow, making error management explicit and ending with more reliable code.

- The `Union<T1, T2>` and all its siblings allows you to create union types. This is pretty similar to the [`OneOf`](https://github.com/mcintyre321/OneOf) library (in fact, heavily inspired in it), but here you will **NOT** be able to get values without checking first.

- The attributes `ResultAttribute` and `UnionAttribute` allow you to generate types and use them with ease.


## Usage (recommended way)
The best way to approach this package is through source generators. This will allow you to define your types with ease and work on your problems straight away.
You will need the `Funzo` and `Funzo.SourceGenerators` packages.
If you want to create a result, you can just create a partial class inheriting from `Result` or `Union` and decorate them with the `Result`/`Union` attribute.

```Csharp
[Union]
public partial class ApiError : Union<Error1, Error2, Error3>;

[Result]
public partial class ApiCallResult : Result<ApiError>;

[Union]
public partial class ItemCreationError : Union<ItemWithNameAlreadyExists, InvalidItemName, EmptyItemName>;

[Result]
public partial class ItemCreatedResult : Result<ItemId, ItemCreationError>;
```


## Documentation

### Unit
Unit is a helper type to represent the absence of a return value (think of it as void). Because in functional programming every function returns a value, it is added here for compatibility.

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

In case you want to do something with the value first, you can use the `Inspect` function:
```Csharp
Option<User> maybeUser = GetUser();

user.Inspect(user => Console.WriteLine($"Retrieved user {user.Name}");
```

### Result

If you want to create a `Result` that will return `Unit`, you can now use the `Result<TErr>` variant. This also removes the need to supply `Unit` to the `Ok` static method.

You can create an instance using the Ok/Err static methods:
```Csharp
var okResult = Result<ProcessError>.Ok();
var errorResult = Result<Unit, ProcessError>.Err(ProcessError.DatabaseConnection);
```

You can map the ok value or err value using `Map` and `MapErr` methods:
```Csharp
var okResult = Result<int, Exception>.Ok(3).Map(x => x*2)); // Ok(6)
var errResult = Result<string>.Err("failure").MapErr(x => x.ToUpper()); // Err("FAILURE")
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

Same as `Option`, we have 2 new methods: `Inspect` and `InspectErr` in case you want to do something with the values without actually changing anything.

### Union
Unions represent a variable that can be of several different types. At the moment, there are only unions for 5 generic types max. This was on purpose, as it usually more than 5 means you need a refactor to group some of them (at least in my opinion). If you need more, feel free to use the `Funzo.Generator` project and change the ordinality to whatever you need.

You can create an instance by using the constructor or by implicitly converting from it:

```Csharp
var union = new Union<int, string, DateTime>("text");

Union<int, string, DateTime> newUnion = 44;
```

To check if the union is from an specific type, you can use the `Is` method:

```Csharp
Union<int, string, DateTime> union = "text";

if (union.Is<string>(out var text))
{
    Console.WriteLine("Union is a string");
}
```

If you want to do different actions depending on the inner value, you can use the `Switch` or the `Match` methods:
```Csharp
Union<int, string, DateTime> union = DateTime.UtcNow;

var typeOfDate = union.Match(
    i => "Unix timestamp",
    s => "ISO string",
    d => "DateTime");

union.Switch(
    i => Console.WriteLine("Unix timestamp"),
    s => Console.WriteLine("ISO string"),
    d => Console.WriteLine("DateTime"))
```

Unions in this package don't have a `.Value` property and they will never have.

## Serialization
The package `Funzo.Serialization` holds the `JsonConverterFactory` classes needed to work with `System.Text.Json`. At the moment, `Option` serializes to:
```JSON
{ "HasValue": bool, "Value": *whatever* }
```
and `Result` to:
```JSON
{ "IsOk": true, "Ok": *whatever* }
or
{ "IsOk": false, "Err": *whatever* }
```

## Example
There is a working example of a payment system under `Funzo.Example`. Feel free to take a look at how the code will look after using it.

## Further work
- [ ] Create a serializer for union types. At the moment, there is nothing planned, as I'm not sure if I should just serialize it to whatever object it is or do something like:
    ```JSON
    {
        "type": "Item1",
        "data": ...
    }
    ```
- [ ] Provide more methods to work with unions. They hold the most complexity, so they should provide the maximum flexibility possible without losing type safety.

## Design philosophy
The idea behind this small package was to provide `Option`/`Result` monads that work idiomatically with C#, whithout losing the essence of them.

In order to achieve this, an approach of *Explicit better than implicit* was used:
- When working with `Option`, minimize the posibility of `NullReferenceException` by limiting the options to get the value out, enforcing the developer to handle all the cases.
- When working with `Result`, minimize the risk of unforseen consequences (λ) by encouraging to use the `Match` statement.
- Unions don't have the possibility of getting the value explicitly, forcing the developer to use the `Is` method or `Switch`/`Match`
- Encourage the usage of Error values, let it be records with some payload or enums, that provide useful information and force the developer to take action for each one of them. By being explicit in what kind of errors can pop out, the developer is forced to handle all the cases than can go wrong and not rely on catch blocks.

## What's missing (updated)
I had to update this list, as with the inclusion of `Union` types a lot can be achieved.
- ~~In order to create a `Result`, both types should be specified. This is annoying, because when you are using large class names, you end up doing:~~
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
    ~~This can be mitigated by using `using` alias like this:~~
    ```Csharp
    using CreateUserResult = Funzo.Result<UseCases.CreateUserReturnValue, UseCases.CreateUserError>;

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
    **This is solved by allowing inheritance with the source generator attribute**

    You can now just do:
    ```Csharp
    [Result]
    public partial class MyProcessResult : Result<Id, UnionError>;
    ```
    And the signature would be:
    ```Csharp
    public async Task<MyProcessResult> Handle(...)
    ```

- ~~`Result<Unit, _>` feels weird, as you have to manually do `Result<Unit, _>.Ok(default)` or `Result<Unit, _>.Ok(new Unit())`. No workaround for this I'm afraid.~~
    
  **This is also solved by using the class `Result<TErr>`** 
- Early returns feel off. I would've loved to have something similar to [Rust's question mark](https://doc.rust-lang.org/rust-by-example/std/result/question_mark.html), but `Option.IsSome` and `Result.IsErr` are the closest things I could think of.
- ~~The absence of `union types`/`discriminated unions`/`closed enums` make managing the different options underwhelming and unreliable if you are not careful. If you have a method like this:~~ 
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
    ~~You may think that you are handling everything, as the enum only has 2 options, but you would receive a warning. 
    This is because enums are `int` in C#, so you could do `(ProcessError)435627` and pass. 
    Their proposed solution is to add a general case `_ => WHATEVER` but this is exactly what this library is trying to avoid. Again, the goal is to be explicit, because if you add a default case, you will not receive a warning nor an error when you add another error in `ProcessError`. So the only option for now is to disable the rule. Ending like this:~~ 
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

    **I'm happy with this one too.** 

    By using the `Union` class you are forced to deal with all the cases without modifying all the methods that use this class. Better result than what I expected.