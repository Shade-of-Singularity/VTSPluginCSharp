// Note: support for this system was dropped until we can implement unresolved aliases.
//  The idea is to replace all UniTasks (or custom task struct) with GDTask before compilation occurs.
//  We will see if it works. For now though, Godot will be left without a plugin.
// Note: it's possible to compile it once or twice - but we need a way, compatible with MSBuild.
//  It's better for maintainability.

//// IMPORTANT! Do NOT rename the file!
//// It is used by deploy scripts!
//#if GDTask
//global using AsyncTaskVoid = GodotTask.GDTaskVoid;
//global using AsyncTask = GodotTask.GDTask;
//global using AsyncBoolTask = Cysharp.Threading.Tasks.UniTask<bool>;
//global using AsyncMemoryTask = Cysharp.Threading.Tasks.UniTask<System.ReadOnlyMemory<byte>>;
//#elif UniTask
//global using AsyncTaskVoid = Cysharp.Threading.Tasks.UniTaskVoid;
//global using AsyncTask = Cysharp.Threading.Tasks.UniTask;
//global using AsyncBoolTask = Cysharp.Threading.Tasks.UniTask<bool>;
//global using AsyncMemoryTask = Cysharp.Threading.Tasks.UniTask<System.ReadOnlyMemory<byte>>;
//global using AsyncVTSResponseDataRequestResultTask = Cysharp.Threading.Tasks.UniTask<VTSPlugin.RequestResult<T>>;
//#endif
