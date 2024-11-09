using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core;

public static class TaskExtension
{
	public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
	{
		using CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();
		if (await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)) == task)
		{
			timeoutCancellationTokenSource.Cancel();
			return await task;
		}
		throw new TimeoutException(string.Format("{0}: The operation has timed out after {1:mm\\:ss}", "TimeoutAfter", timeout));
	}
}
