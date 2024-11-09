using System;
using System.Net;

namespace Code.Core.Network;

internal static class EndianConverter
{
	public static short NetworkToHostOrder(short value)
	{
		return IPAddress.NetworkToHostOrder(value);
	}

	public static int NetworkToHostOrder(int value)
	{
		return IPAddress.NetworkToHostOrder(value);
	}

	public static long NetworkToHostOrder(long value)
	{
		return IPAddress.NetworkToHostOrder(value);
	}

	public static short HostToNetworkOrder(short value)
	{
		return IPAddress.HostToNetworkOrder(value);
	}

	public static int HostToNetworkOrder(int value)
	{
		return IPAddress.HostToNetworkOrder(value);
	}

	public static long HostToNetworkOrder(long value)
	{
		return IPAddress.HostToNetworkOrder(value);
	}

	public static float NetworkToHostOrder(float value)
	{
		if (BitConverter.IsLittleEndian)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return BitConverter.ToSingle(bytes, 0);
		}
		return value;
	}

	public static double NetworkToHostOrder(double value)
	{
		if (BitConverter.IsLittleEndian)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return BitConverter.ToDouble(bytes, 0);
		}
		return value;
	}

	public static float HostToNetworkOrder(float value)
	{
		if (BitConverter.IsLittleEndian)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return BitConverter.ToSingle(bytes, 0);
		}
		return value;
	}

	public static double HostToNetworkOrder(double value)
	{
		if (BitConverter.IsLittleEndian)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return BitConverter.ToDouble(bytes, 0);
		}
		return value;
	}
}
