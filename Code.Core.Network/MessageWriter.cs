using System.IO;
using System.Text;

namespace Code.Core.Network;

public class MessageWriter
{
	private readonly MemoryStream _bufferMs;

	private readonly BinaryWriter _bufferBw;

	private BinaryReader _bufferBr;

	public static short GetSize(string data)
	{
		return (short)Encoding.UTF8.GetByteCount(data);
	}

	public MessageWriter(int size)
	{
		_bufferMs = new MemoryStream(size);
		_bufferBw = new BinaryWriter(_bufferMs);
	}

	public byte[] GetData()
	{
		return _bufferMs.ToArray();
	}

	public void WriteInt8(sbyte data)
	{
		_bufferBw.Write(data);
	}

	public void WriteInt16(short data)
	{
		_bufferBw.Write(EndianConverter.HostToNetworkOrder(data));
	}

	public void WriteInt32(int data)
	{
		_bufferBw.Write(EndianConverter.HostToNetworkOrder(data));
	}

	public void WriteInt64(long data)
	{
		_bufferBw.Write(EndianConverter.HostToNetworkOrder(data));
	}

	public void WriteFloat(float data)
	{
		_bufferBw.Write(EndianConverter.HostToNetworkOrder(data));
	}

	public void WriteDouble(double data)
	{
		_bufferBw.Write(EndianConverter.HostToNetworkOrder(data));
	}

	public void WriteString(string data)
	{
		WriteInt16((short)Encoding.UTF8.GetByteCount(data));
		_bufferBw.Write(Encoding.UTF8.GetBytes(data));
	}

	public void WriteBytes(byte[] data)
	{
		WriteInt16((short)data.Length);
		_bufferBw.Write(data);
	}

	public void WriteOpCode(int data)
	{
		WriteInt16((short)data);
	}

	public void WriteTypeCode(int data)
	{
		WriteInt16((short)data);
	}
}
