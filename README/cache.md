# Cache

One of [ASP.NET Core Best Practices](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices) is to [cache agressively](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-8.0#cache-aggressively).

> [!NOTE]
> In computing, a [cache]() is a hardware or software component that stores data so that future requests for that data can be served faster; the data stored in a cache might be the result of an earlier computation or a copy of data stored elsewhere. A **cache hit** occurs when the requested data can be found in a cache, while a **cache miss** occurs when it cannot. Cache hits are served by reading data from the cache, which is faster than recomputing a result or reading from a slower data store; thus, the more requests that can be served from the cache, the faster the system performs.

ASP.NET Core supports several [types of caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview?view=aspnetcore-8.0):

- In-memory caching
- Distributed Cache
- HybridCache

We're gonna apply [in-memory caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-8.0). There are two versions of this:

- [System.Runtime.Caching/MemoryCache](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.caching.memorycache) (we need to install a [NuPackage](https://www.nuget.org/packages/System.Runtime.Caching/) to use it).
- `IMemoryCache` which is included in [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory/)

We'll use `IMemoryCache` because is recommended over `System.Runtime.Caching/MemoryCache` because it's better integrated into **ASP.NET Core** (works natively with ASP.NET Core **dependency injection**).

## Installing the In-Memory Caching Extension

To use `IMemoryCache` we need to install a [NuPackage](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory/):
```
dotnet add package Microsoft.Extensions.Caching.Memory --version 8.0.0
```

> I actually used the **NuGet Gallery** VS Code extension to install it (searched for `IMemoryCache`).

## Injecting it

In-memory caching is a **service** that's referenced from an app using **Dependency Injection**; we need to request the `IMemoryCache` instance in the `FibonacciController` constructor.

## Registering it

We also have to register the cache service in the container (add it to the builder in `Program.cs`), and restart the server because `dotnet watch` can't handle so much heat ))

## Using it

I made some design choices that could be up for discussion:

- Using the `startIndex` and `endIndex` as cache keys. I guess in a real app, we could use some user id so users don't have access to other users cache results?
- When a **complete subsequence** is generated, I always store them, regardless if `useCache` was true or not.
- I configured the expiration time to **5 minutes**, don't know what's the "standard" (never used cache before).