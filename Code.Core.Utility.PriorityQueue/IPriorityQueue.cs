using System;

namespace Code.Core.Utility.PriorityQueue;

public interface IPriorityQueue<T, in TKey> where TKey : IComparable<TKey>
{
	void Insert(T item, TKey priority);

	T Top();

	T Pop();

	bool IsEmpty();
}
