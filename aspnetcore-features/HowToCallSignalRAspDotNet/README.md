# How to Call SignalR Hub from Controller in ASP.NET Core

A sample ASP.NET Core 8 Web API that demonstrates how to push real-time
messages from a controller to connected SignalR clients using
`IHubContext`. Two controller variants are included — one using a
**strongly-typed hub** (`IHubContext<RandomizerHub, IRandomizerClient>`)
and one using the **non-generic hub** (`IHubContext<RandomizerHub>`) with
`SendAsync`.

> Companion article: [How to Call SignalR Hub from Controller in ASP.NET Core](https://code-maze.com/aspnetcore-call-signalr-from-controller/)

## What it does

Both `GET` endpoints generate a random even number between 2 and 100 and
broadcast it to all connected SignalR clients via the `RandomizerHub`
every 4 seconds for up to 60 seconds. A singleton `TimerManager` ensures
the broadcast loop runs only once per first request.

- **`GET /api/Randomizer/SendRandomNumber`** — strongly-typed hub call
  (`Clients.All.SendClientRandomEvenNumber(value)`)
- **`GET /api/RandomizerWithNonGenericHub/SendRandomNumber`** —
  non-generic hub call (`Clients.All.SendAsync("SendClientRandomEvenNumber", value)`)
- **SignalR hub:** `/numberhub`

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Run

From the solution root:

```bash
dotnet build HowToCallSignalRAspDotNet.slnx
dotnet run --project HowToCallSignalRAspDotNet
```

The default `https` launch profile binds to
`https://localhost:5000;http://localhost:5001` and the browser will be
redirected to `/swagger` when launched from Visual Studio. From the
command line, open <https://localhost:5000/swagger>.

To exercise the real-time path, open a SignalR client (browser, .NET
client, or `curl`-equivalent WebSocket) to `/numberhub`, then hit either
controller endpoint from a second terminal:

```bash
curl https://localhost:5001/api/Randomizer/SendRandomNumber
# Watch the connected SignalR client — it will receive a number every 4s
```

## Project structure

```
HowToCallSignalRAspDotNet/
├── HowToCallSignalRAspDotNet.slnx
├── HowToCallSignalRAspDotNet/
│   ├── Controllers/
│   │   ├── RandomizerController.cs                # strongly-typed hub
│   │   └── RandomizerWithNonGenericHubController.cs # non-generic hub
│   ├── HubConfig/
│   │   └── RandomizerHub.cs                       # Hub<IRandomizerClient>
│   ├── Models/
│   │   └── IRandomizerClient.cs                   # client contract
│   ├── TimerFeatures/
│   │   └── TimerManager.cs                        # broadcast scheduler
│   ├── Program.cs                                 # composition root
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── HowToCallSignalRAspDotNet.csproj
└── Tests/
    ├── RandomizerControllerFactory.cs             # Moq-based fixtures
    ├── RandomizerControllerUnitTest.cs
    ├── RandomizerWithNonGenericHubControllerTest.cs
    ├── GlobalUsings.cs
    └── Tests.csproj
```

## Swagger / OpenAPI

The project registers Swashbuckle via `AddEndpointsApiExplorer()` and
`AddSwaggerGen()`, and exposes the spec & UI in the pipeline:

- **Swagger UI:** `/swagger`
- **OpenAPI JSON:** `/swagger/v1/swagger.json`

Both controllers are listed automatically.

## CORS

The `CorsPolicy` is intentionally permissive so any host can call the
API:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
```

`SetIsOriginAllowed(_ => true)` is used in place of `AllowAnyOrigin()`
because `.AllowAnyOrigin()` cannot be combined with `.AllowCredentials()`
— the framework throws `InvalidOperationException` at startup. The
predicate runs per request, so each origin is accepted individually and
credentials still flow. **Do not use this policy in production** —
restrict to known origins.

## Tests

The `Tests` project uses xUnit + Moq to fake the SignalR hub context and
verify that calling `SendRandomNumber` returns `200 OK` with an `int` and
triggers the expected client invocation. The shared fixtures live in
`RandomizerControllerFactory`.

```bash
dotnet test Tests/Tests.csproj
```
