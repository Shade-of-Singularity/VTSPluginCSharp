# VTSPlugin
Refreshed VTube Studio Plugin implementation for different environments. 99% human-made.

Libraries focus on:
- Ease of use & development,
- Reliability,
- and Performance.

**Status**: Work in Progress.

If you would like this library to move to LTS (Long Term Support) stage quicker - consider giving it a try, and leaving a feedback.
It will motivate us, and show that there's a demand for finalizing the library.

# Limitations
1. AOT (Ahead of time) compilation is not supported.
    - For Unity projects, this means that you cannot publish your app with IL2CPP preset.
    - Same for Godot. AOT is not supported yet.

# Features
1. Simple in use API featuring `async` methods.
2. `UniTask`/`GDTask`-based operations for maximum performance.
3. Specialized libraries for common environments: WPF, Console (WIP), Unity (WIP), Godot (WIP).
4. High performance via packet pooling and precomputed JSON message templates.

If you will find our API or optimizations not applicable for your project, or simply inconvenient - consider trying VTS-Sharp:
https://github.com/FomTarro/VTS-Sharp

VTS Sharp - is also a library for communicating with VTubeStudio, and it is battle tested in countless projects.
Consider giving them a star on GitHub!

# Environments and Libraries
Here are all supported (* - or planned to be supported) environments are listed.

## VTSPlugin.CSharp (Core library)
- Link: -URI-
- Provides API, shared between all libraries.
- Made purely to not write stuff tw-... 4 times over.
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
## DenchiSoft (VTubeStudio)
Thanks to their work, VTubing is accessible as never before.
Sincerely thank you from the bottom of my heart!

And thanks to them maintaining a neat API - we are also able to make plugins, and do whatever we want with our models.
Only thanks to them this is a possibility. Thank you!

Make sure to star their GitHub project is you haven't already:
https://github.com/DenchiSoft/VTubeStudio

## FomTarro (VTS-Sharp)
Thanks to their amazing work for years, a lot of plugins people are using every day are even possible to make and maintain!
VBridger, T.I.T.S, VTS-Heartrate, and so many more!

Their work is amazing, and the API is very clean!
Consider giving them a star on GitHub! Their work deserves it.
https://github.com/FomTarro/VTS-Sharp

## TomaszRewak / C# Stack only JSON parser
Their zero heap allocation JSON parser implementation is amazing!
Their approach inspired a creation of a JsonResult{T} struct.

DarkJune:
I couldn't find a way to make StackOnlyJSONParser not deserialize specific fields for lazy initialization,
so at the end of the day I had to drop the library and use Utf8JsonParser manually.

Never-the-less, the inspiration is obvious, and their library is **absolutely amazing**!
Can't recommend it enough! Allows avoiding so much headache while keeping everything performant.

**Give them a star**: https://github.com/TomaszRewak/C-sharp-stack-only-json-parser