# VTSPlugin
Refreshed VTube Studio Plugin implementation for different environments.

Libraries focus on:
- Ease of use & development,
- Reliability,
- and Performance.

**Status**: Work in Progress.

If you would like this library to move to LTS (Long Term Support) stage quicker - consider giving it a try, and leaving a feedback.
It will motivate us, and show that there's a demand for finalizing the library.

# AI usage
None.

# Table of Contents:
1. [Limitations](#limitations)
2. [Features](#features)
3. [Supported Environments and Libraries](#supported-environments-an-libraries)
4. [Acknowledgements](#acknowledgements)
    1. [DenchiSoft / VTubeStudio](#denchisoft-vtubestudio)
    1. [FomTarro / VTS-Sharp](#fomtarro-vts-sharp)
    1. [TomaszRewak / Stack only JSON parser](#tomaszrewak-stack-only-json-parser)
5. [API (Requests)](#api-requests)
6. [API (Events)](#api-events)
7. [WIP](#wip)

# Limitations
1. AOT (Ahead of time) compilation is not supported **yet**.
    - For Unity projects, this means that you cannot publish your app with IL2CPP preset.
    - Same for Godot. AOT is not supported yet.
    - IsNativeAOT cannot be used in WPF and any C# apps.
AOT is planned to be supported in the future.

# Features
1. Good-old `Delegate`-based requests.
2. New **heap zero-allocation** requests.
3. `async` `UniTask`/`GDTask`-based for flexibility.
4. High performance via packet pooling and precomputed JSON message templates.
5. Specialized libraries for common environments: WPF, Console (WIP), Unity (WIP), Godot (WIP).

If you will find our API or optimizations not applicable for your project, or simply inconvenient - consider trying VTS-Sharp:
https://github.com/FomTarro/VTS-Sharp

VTS Sharp - is also a library for communicating with VTubeStudio, and it is battle tested in countless projects.
Consider giving them a star on GitHub!

# Supported Environments and Libraries
Here are all supported (* - or planned to be supported) environments are listed.

## VTSPlugin (Core library)
- Link: -URI-
- Bare-bone library, targeting non-natively supported envornments.
- Contains only API, shared between all libraries.
## VTSPlugin.WPF (Windows Presentation Foundation)
- Link: -URI-
- Implementation features callbacks via `INotifyPropertyChanged`.
- Heavily recommend to check this guide for WPF as well: https://metashapes.com/blog/not-shooting-foot-wpf-best-practices/
- Uses C# implementation of `UniTask` for the best performance.
## VTSPlugin.Console *
- Link: -URI-
- The most lightweight implementation, supporting multi-threading.
- Features only technical callbacks, omitting any UI callbacks.
## VTSPlugin.Unity *
- Link: -URI-
- Compiled inside of your editor project.
- Requires `UniTask.unitypackage` to be installed to compile properly: https://github.com/Cysharp/UniTask/releases/tag/2.5.11
- Omits concurrency handling (accessing objects is unsafe from multiple threads) in favor of UI performance (?)
- Custom UnityEditor for ready-to-use service/studio implementations.
## VTSPlugin.Godot *
- Link: -URI-
- Uses `GDTask` - `UniTask`-based implementation for tasks, making `async` calls a lot more performant than usual.
- Can be installed via a NuGet package: -URI-
- Also optional GodotEditor package: -URI-

Note: Godot's GDScript environment is not supported yet.
There are no plans to support it either, as there is no demand.
Please let us know if you need it.

# Acknowledgements
## DenchiSoft / VTubeStudio
Thanks to their work, VTubing is accessible as never before.
Sincerely thank you from the bottom of my heart!

And thanks to them maintaining a neat API - we are also able to make plugins, and do whatever we want with our models.
Only thanks to them this is a possibility. Thank you!

Make sure to star their GitHub project is you haven't already:
https://github.com/DenchiSoft/VTubeStudio

## FomTarro / VTS-Sharp
Thanks to their amazing work for years, a lot of plugins people are using every day are even possible to make and maintain!
VBridger, T.I.T.S, VTS-Heartrate, and so many more!

Their work is amazing, and the API is very clean!
Consider giving them a star on GitHub! Their work deserves it.
https://github.com/FomTarro/VTS-Sharp

## TomaszRewak / Stack only JSON parser
Their zero heap allocation JSON parser implementation is amazing!
Their approach inspired a creation of a JsonResult{T} struct.

**Give them a star**: https://github.com/TomaszRewak/C-sharp-stack-only-json-parser

# API (Requests)
API features 3 ways of requesting things and handling them.
## Delegate-based requests.
`Delegate`-based requests are inspired by [VTS-Sharp](https://github.com/FomTarro/VTS-Sharp)

They are alright to use in most cases, and with right approach - are faster than any other approach in the library.

You can check our benchmarks: [VTSPlugin.API.Benchmarks.cs](https://gist.github.com/DarkJune0122/573e16527e0cc2a3ec524f3a4be8a119)

Most common way to use them is:
```C#
// Local variables:
static VTSConnection Connection { get; set; }

// Request / call-site:
public void Requests()
{
    // Note: VTSService also has the same API.
    Connection.RequestAPI(onSuccess: HandleSuccess, onError: HandleError);
}

bool HandleSuccess(VTSAPIStateResponseData data)
{
    IVTSLogger.Default.LogInfo($"Success! {data}");
    return true; // true - returns 'data' to the pool. Return false if you want to store it somewhere.
                 // Note: this API might change in the future.
}

void HandleError(ErrorID errorID, string errorMessage)
{
    IVTSLogger.Default.LogError($"{errorID}: {errorMessage}");
}
```
More advanced way is to avoid [closures](https://stackoverflow.com/questions/428617/what-are-closures-in-net):
```C#
public class MyClass
{
    // Local variables:
    static VTSConnection Connection { get; set; }
    
    // Request / call-site:
    public void Requests()
    {
        // Note: VTSService also has the same API.
        Connection.RequestAPI(this, onSuccess: HandleSuccess, onError: HandleError);
    }
    
    static bool HandleSuccess(MyClass cls, VTSAPIStateResponseData data)
    {
        IVTSLogger.Default.LogInfo($"{cls} handled successful request! {data}");
        return true; // true - returns 'data' to the pool. Return false if you want to store it somewhere.
                     // Note: this API might change in the future.
    }
    
    static void HandleError(MyClass cls, ErrorID errorID, string errorMessage)
    {
        IVTSLogger.Default.LogError($"{cls} handled failed request! {errorID}: {errorMessage}");
    }
}
```

## Async-based requests.
`async`-based requests use `UniTask` or `GDTask`, depending on the environment.

They both provide heap zero-allocation natively, and _usually_ be faster than delegate-based requests.

You can check our benchmarks: [VTSPlugin.API.Benchmarks.cs](https://gist.github.com/DarkJune0122/573e16527e0cc2a3ec524f3a4be8a119)

Usage looks like this:
(in Godot, mentally replace `UniTask` with `GDTask`)
```C#
// Local variables:
static VTSConnection Connection { get; set; }

// Request / call-site:
public async UniTaskVoid Requests()
{
    // Note: VTSService also has the same API.
    using var result = await Connection.RequestAPIAsync();
    if (result.IsSuccessful(out var data))
    {    
        IVTSLogger.Default.LogInfo($"Success! {data}");
    }
    else
    {
        IVTSLogger.Default.LogError($"{errorID}: {errorMessage}");
    }
}
```

## Ref struct zero-allocation requests.
This aproach is more verbose, but ideal for people needing lowest possible memory footpring of the program.

It's important to note that `async`-based requests are already heap zero-allocation.

It has limitations:
1. `ref struct`s force you to "consume" the entire packet from VTube Studio on-the-spot.
2. `ref struct`s cannot be stored in most fields - you will need to manually unpack them.
3. You cannot dispatch `ref sturct`s to main thread without manual work.  

Be considerate when using it! Thank you.
```C#
// Local variables:
static VTSConnection Connection { get; set; }

// Request / call-site:
public void Requests()
{
    // Note: VTSService also has the same API.
    Connection.StackRequestAPI(onSuccess: HandleSuccess, onError: HandleError);
}

static void HandleSuccess(ref Utf8JsonReader reader)
{
    VTSStackAPIStateResponse response = new(ref reader);
    IVTSLogger.Default.LogInfo($"Success! {response.ToString()}");
}

static void HandleError(ErrorID errorID, string errorMessage)
{
    IVTSLogger.Default.LogError($"{errorID}: {errorMessage}");
}
```
Or with generic state:
```C#
public class MyClass
{
    // Local variables:
    static VTSConnection Connection { get; set; }
    
    // Request / call-site:
    public void Requests()
    {
        // Note: VTSService also has the same API.
        Connection.StackRequestAPI(this, onSuccess: HandleSuccess, onError: HandleError);
    }
    
    static void HandleSuccess(MyClass cls, ref Utf8JsonReader reader)
    {
        VTSStackAPIStateResponse response = new(ref reader);
        IVTSLogger.Default.LogInfo($"{cls} handled successful request! {response.ToString()}");
    }
    
    static void HandleError(MyClass cls, ErrorID errorID, string errorMessage)
    {
        IVTSLogger.Default.LogError($"{cls} handled failed request! {errorID}: {errorMessage}");
    }
}
```

# API (Events)
API for events is not realized yet.
It will likely be similar to Requests API.

# WIP
Currently, following is in development:
1. API for handling VTube Studio events.
2. Zero-allocation for requests, requiring you to list multiple inputs, like subscribe to events request.
