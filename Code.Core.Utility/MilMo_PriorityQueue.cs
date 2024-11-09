using System.Collections.Generic;

namespace Code.Core.Utility;

public class MilMo_PriorityQueue<T>
{
	private readonly Queue<T> _highQueue = new Queue<T>();

	private readonly Queue<T> _normalQueue = new Queue<T>();

	private readonly Queue<T> _lowQueue = new Queue<T>();

	public int Count => _highQueue.Count + _normalQueue.Count + _lowQueue.Count;

	public void Enqueue(T element, MilMo_Priority priority = MilMo_Priority.Normal)
	{
		switch (priority)
		{
		case MilMo_Priority.Normal:
			_normalQueue.Enqueue(element);
			break;
		case MilMo_Priority.Low:
			_lowQueue.Enqueue(element);
			break;
		case MilMo_Priority.High:
			_highQueue.Enqueue(element);
			break;
		default:
			_normalQueue.Enqueue(element);
			break;
		}
	}

	public T Dequeue()
	{
		if (_highQueue.Count > 0)
		{
			return _highQueue.Dequeue();
		}
		if (_normalQueue.Count > 0)
		{
			return _normalQueue.Dequeue();
		}
		if (_lowQueue.Count > 0)
		{
			return _lowQueue.Dequeue();
		}
		return default(T);
	}
}
