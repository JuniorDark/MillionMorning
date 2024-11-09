using System.Collections.Generic;
using System.Linq;

namespace Core.Utilities;

internal class MilMoPrioQueue<T> where T : IHasPriorityKey
{
	private readonly List<T> _queue = new List<T>();

	private T Highest { get; set; }

	public void Enqueue(T element)
	{
		_queue.Add(element);
		RecalculateQueue();
	}

	private void SortQueue()
	{
		_queue.Sort((T a, T b) => b.GetPriority() - a.GetPriority());
	}

	public T Peek()
	{
		return Highest;
	}

	private void RecalculateQueue()
	{
		SortQueue();
		Highest = _queue.FirstOrDefault();
	}

	public T Pop()
	{
		_queue.Remove(Highest);
		T highest = Highest;
		Highest = _queue.FirstOrDefault();
		return highest;
	}

	public bool Contains(T element)
	{
		return _queue.Any((T e) => e.Equals(element));
	}

	public bool IsEmpty()
	{
		return _queue.Count < 1;
	}

	public bool HasHigherPriority(T element)
	{
		if (element != null)
		{
			return Highest.GetPriority() > element.GetPriority();
		}
		return true;
	}
}
