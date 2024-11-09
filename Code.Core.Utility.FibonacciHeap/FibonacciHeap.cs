using System;
using System.Collections.Generic;

namespace Code.Core.Utility.FibonacciHeap;

public class FibonacciHeap<T, TKey> : IFibonacciHeap<T, TKey> where TKey : IComparable<TKey>
{
	private readonly double _oneOverLogPhi = 1.0 / Math.Log((1.0 + Math.Sqrt(5.0)) / 2.0);

	private FibonacciHeapNode<T, TKey> _minNode;

	private int _nodesCount;

	private readonly TKey _minKeyValue;

	public FibonacciHeap(TKey minKeyValue)
	{
		_minKeyValue = minKeyValue;
	}

	public bool IsEmpty()
	{
		return _minNode == null;
	}

	public void Clear()
	{
		_minNode = null;
		_nodesCount = 0;
	}

	public void DecreaseKey(FibonacciHeapNode<T, TKey> x, TKey k)
	{
		if (k.CompareTo(x.Key) > 0)
		{
			throw new ArgumentException("decreaseKey() got larger key value");
		}
		x.Key = k;
		FibonacciHeapNode<T, TKey> parent = x.Parent;
		if (parent != null && x.Key.CompareTo(parent.Key) < 0)
		{
			Cut(x, parent);
			CascadingCut(parent);
		}
		if (x.Key.CompareTo(_minNode.Key) < 0)
		{
			_minNode = x;
		}
	}

	public void Delete(FibonacciHeapNode<T, TKey> x)
	{
		DecreaseKey(x, _minKeyValue);
		RemoveMin();
	}

	public void Insert(FibonacciHeapNode<T, TKey> node)
	{
		if (_minNode != null)
		{
			node.Left = _minNode;
			node.Right = _minNode.Right;
			_minNode.Right = node;
			node.Right.Left = node;
			if (node.Key.CompareTo(_minNode.Key) < 0)
			{
				_minNode = node;
			}
		}
		else
		{
			_minNode = node;
		}
		_nodesCount++;
	}

	public FibonacciHeapNode<T, TKey> Min()
	{
		return _minNode;
	}

	public FibonacciHeapNode<T, TKey> RemoveMin()
	{
		FibonacciHeapNode<T, TKey> minNode = _minNode;
		if (minNode != null)
		{
			int num = minNode.Degree;
			FibonacciHeapNode<T, TKey> fibonacciHeapNode = minNode.Child;
			while (num > 0)
			{
				FibonacciHeapNode<T, TKey> right = fibonacciHeapNode.Right;
				fibonacciHeapNode.Left.Right = fibonacciHeapNode.Right;
				fibonacciHeapNode.Right.Left = fibonacciHeapNode.Left;
				fibonacciHeapNode.Left = _minNode;
				fibonacciHeapNode.Right = _minNode.Right;
				_minNode.Right = fibonacciHeapNode;
				fibonacciHeapNode.Right.Left = fibonacciHeapNode;
				fibonacciHeapNode.Parent = null;
				fibonacciHeapNode = right;
				num--;
			}
			minNode.Left.Right = minNode.Right;
			minNode.Right.Left = minNode.Left;
			if (minNode == minNode.Right)
			{
				_minNode = null;
			}
			else
			{
				_minNode = minNode.Right;
				Consolidate();
			}
			_nodesCount--;
		}
		return minNode;
	}

	public int Size()
	{
		return _nodesCount;
	}

	public static FibonacciHeap<T, TKey> Union(FibonacciHeap<T, TKey> h1, FibonacciHeap<T, TKey> h2)
	{
		FibonacciHeap<T, TKey> fibonacciHeap = new FibonacciHeap<T, TKey>((h1._minKeyValue.CompareTo(h2._minKeyValue) < 0) ? h1._minKeyValue : h2._minKeyValue)
		{
			_minNode = h1._minNode
		};
		if (fibonacciHeap._minNode != null)
		{
			if (h2._minNode != null)
			{
				fibonacciHeap._minNode.Right.Left = h2._minNode.Left;
				h2._minNode.Left.Right = fibonacciHeap._minNode.Right;
				fibonacciHeap._minNode.Right = h2._minNode;
				h2._minNode.Left = fibonacciHeap._minNode;
				if (h2._minNode.Key.CompareTo(h1._minNode.Key) < 0)
				{
					fibonacciHeap._minNode = h2._minNode;
				}
			}
		}
		else
		{
			fibonacciHeap._minNode = h2._minNode;
		}
		fibonacciHeap._nodesCount = h1._nodesCount + h2._nodesCount;
		return fibonacciHeap;
	}

	private void CascadingCut(FibonacciHeapNode<T, TKey> y)
	{
		FibonacciHeapNode<T, TKey> parent = y.Parent;
		if (parent != null)
		{
			if (!y.Mark)
			{
				y.Mark = true;
				return;
			}
			Cut(y, parent);
			CascadingCut(parent);
		}
	}

	private void Consolidate()
	{
		int num = (int)Math.Floor(Math.Log(_nodesCount) * _oneOverLogPhi) + 1;
		List<FibonacciHeapNode<T, TKey>> list = new List<FibonacciHeapNode<T, TKey>>(num);
		for (int i = 0; i < num; i++)
		{
			list.Add(null);
		}
		int num2 = 0;
		FibonacciHeapNode<T, TKey> fibonacciHeapNode = _minNode;
		if (fibonacciHeapNode != null)
		{
			num2++;
			for (fibonacciHeapNode = fibonacciHeapNode.Right; fibonacciHeapNode != _minNode; fibonacciHeapNode = fibonacciHeapNode.Right)
			{
				num2++;
			}
		}
		while (num2 > 0)
		{
			int num3 = fibonacciHeapNode.Degree;
			FibonacciHeapNode<T, TKey> right = fibonacciHeapNode.Right;
			while (true)
			{
				FibonacciHeapNode<T, TKey> fibonacciHeapNode2 = list[num3];
				if (fibonacciHeapNode2 == null)
				{
					break;
				}
				if (fibonacciHeapNode.Key.CompareTo(fibonacciHeapNode2.Key) > 0)
				{
					FibonacciHeapNode<T, TKey> fibonacciHeapNode3 = fibonacciHeapNode2;
					fibonacciHeapNode2 = fibonacciHeapNode;
					fibonacciHeapNode = fibonacciHeapNode3;
				}
				Link(fibonacciHeapNode2, fibonacciHeapNode);
				list[num3] = null;
				num3++;
			}
			list[num3] = fibonacciHeapNode;
			fibonacciHeapNode = right;
			num2--;
		}
		_minNode = null;
		for (int j = 0; j < num; j++)
		{
			FibonacciHeapNode<T, TKey> fibonacciHeapNode4 = list[j];
			if (fibonacciHeapNode4 == null)
			{
				continue;
			}
			if (_minNode != null)
			{
				fibonacciHeapNode4.Left.Right = fibonacciHeapNode4.Right;
				fibonacciHeapNode4.Right.Left = fibonacciHeapNode4.Left;
				fibonacciHeapNode4.Left = _minNode;
				fibonacciHeapNode4.Right = _minNode.Right;
				_minNode.Right = fibonacciHeapNode4;
				fibonacciHeapNode4.Right.Left = fibonacciHeapNode4;
				if (fibonacciHeapNode4.Key.CompareTo(_minNode.Key) < 0)
				{
					_minNode = fibonacciHeapNode4;
				}
			}
			else
			{
				_minNode = fibonacciHeapNode4;
			}
		}
	}

	private void Cut(FibonacciHeapNode<T, TKey> x, FibonacciHeapNode<T, TKey> y)
	{
		x.Left.Right = x.Right;
		x.Right.Left = x.Left;
		y.Degree--;
		if (y.Child == x)
		{
			y.Child = x.Right;
		}
		if (y.Degree == 0)
		{
			y.Child = null;
		}
		x.Left = _minNode;
		x.Right = _minNode.Right;
		_minNode.Right = x;
		x.Right.Left = x;
		x.Parent = null;
		x.Mark = false;
	}

	public void Link(FibonacciHeapNode<T, TKey> newChild, FibonacciHeapNode<T, TKey> newParent)
	{
		newChild.Left.Right = newChild.Right;
		newChild.Right.Left = newChild.Left;
		newChild.Parent = newParent;
		if (newParent.Child == null)
		{
			newParent.Child = newChild;
			newChild.Right = newChild;
			newChild.Left = newChild;
		}
		else
		{
			newChild.Left = newParent.Child;
			newChild.Right = newParent.Child.Right;
			newParent.Child.Right = newChild;
			newChild.Right.Left = newChild;
		}
		newParent.Degree++;
		newChild.Mark = false;
	}
}
