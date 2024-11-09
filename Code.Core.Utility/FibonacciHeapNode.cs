using System;

namespace Code.Core.Utility;

public class FibonacciHeapNode<T, TKey> where TKey : IComparable<TKey>
{
	public T Data { get; set; }

	public FibonacciHeapNode<T, TKey> Child { get; set; }

	public FibonacciHeapNode<T, TKey> Left { get; set; }

	public FibonacciHeapNode<T, TKey> Parent { get; set; }

	public FibonacciHeapNode<T, TKey> Right { get; set; }

	public bool Mark { get; set; }

	public TKey Key { get; set; }

	public int Degree { get; set; }

	public FibonacciHeapNode(T data, TKey key)
	{
		Right = this;
		Left = this;
		Data = data;
		Key = key;
	}
}
