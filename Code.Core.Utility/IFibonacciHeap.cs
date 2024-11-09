using System;

namespace Code.Core.Utility;

public interface IFibonacciHeap<T, TKey> where TKey : IComparable<TKey>
{
	void Clear();

	void DecreaseKey(FibonacciHeapNode<T, TKey> x, TKey k);

	void Delete(FibonacciHeapNode<T, TKey> x);

	void Insert(FibonacciHeapNode<T, TKey> node);

	bool IsEmpty();

	FibonacciHeapNode<T, TKey> Min();

	FibonacciHeapNode<T, TKey> RemoveMin();

	int Size();
}
