// IMPORTANT! Do NOT rename the file!
// It is used by deploy scripts!
#if GDTask
global using AsyncTaskVoid = GodotTask.GDTaskVoid;
global using AsyncTask = GodotTask.GDTask;
global using AsyncBoolTask = Cysharp.Threading.Tasks.UniTask<bool>;
global using AsyncMemoryTask = Cysharp.Threading.Tasks.UniTask<System.ReadOnlyMemory<byte>>;
#elif UniTask
global using AsyncTaskVoid = Cysharp.Threading.Tasks.UniTaskVoid;
global using AsyncTask = Cysharp.Threading.Tasks.UniTask;
global using AsyncBoolTask = Cysharp.Threading.Tasks.UniTask<bool>;
global using AsyncMemoryTask = Cysharp.Threading.Tasks.UniTask<System.ReadOnlyMemory<byte>>;
#endif
