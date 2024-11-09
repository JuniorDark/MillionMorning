using System;
using Code.Core.Utility.FibonacciHeap;

namespace Code.Core.Utility.PriorityQueue;

public class PriorityQueue<TElement, TPriority> : IPriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
	private readonly FibonacciHeap<TElement, TPriority> _heap;

	public PriorityQueue(TPriority minPriority)
	{
		_heap = new FibonacciHeap<TElement, TPriority>(minPriority);
	}

	public void Insert(TElement item, TPriority priority)
	{
		_heap.Insert(new FibonacciHeapNode<TElement, TPriority>(item, priority));
	}

	public TElement Top()
	{
		return _heap.Min().Data;
	}

	public TElement Pop()
	{
		return _heap.RemoveMin().Data;
	}

	public bool IsEmpty()
	{
		return _heap.IsEmpty();
	}
}
