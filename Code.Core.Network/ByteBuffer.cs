using System;

namespace Code.Core.Network;

public class ByteBuffer
{
	private int _limit;

	public byte[] Bytes { get; private set; }

	public int Pos { get; private set; }

	public bool HasRemaining => Remaining() > 0;

	public ByteBuffer(int capacity)
	{
		Bytes = new byte[capacity];
		Pos = 0;
		_limit = 0;
	}

	public void Put(byte[] bytes, int length)
	{
		Array.Copy(bytes, 0, Bytes, Pos, length);
		Pos += length;
	}

	public int Remaining()
	{
		return _limit - Pos;
	}

	public void Skip(int length)
	{
		Pos += length;
	}

	public void Flip()
	{
		_limit = Pos;
		Pos = 0;
	}

	public void Compact()
	{
		if (Pos == 0)
		{
			Pos = _limit;
			return;
		}
		int num = Remaining();
		if (num > 0)
		{
			Array.Copy(Bytes, Pos, Bytes, 0, num);
		}
		Pos = num;
		_limit = num;
	}
}
