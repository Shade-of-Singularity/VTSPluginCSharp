using Cysharp.Threading.Tasks;
using System.Text.Json;

namespace VTSPlugin;

/// <summary>
/// Examples for using <see cref="RequestResult{T}"/>.
/// </summary>
public static class RequestResultUsageExample
{
	static readonly IVTSConnection Connection = null!;
	public static async UniTaskVoid Modern()
	{
		using var result = await Connection.RequestAPIState();
		if (result.IsSuccessful(out var data))
		{
			Console.WriteLine($"Request successful! Data: {data}");
		}
		else
		{
			Console.WriteLine($"Request failed! ErrorID: {result.ErrorID}, Message: {result.ErrorMessage.AsUTF8String()}");
		}
	}

	public static async UniTaskVoid ModernViaStruct()
	{
		using var result = await Connection.RequestAPIState();
		if (result.Successful)
		{
			Console.WriteLine($"Request successful! Data: {result.Data}");
		}
		else
		{
			Console.WriteLine($"Request failed! ErrorID: {result.ErrorID}, Message: {result.ErrorMessage.AsUTF8String()}");
		}
	}
}
