namespace Code.Core.Network.types;

public class SoundPoint
{
	private readonly float _time;

	private readonly string _path;

	public SoundPoint(MessageReader reader)
	{
		_time = reader.ReadFloat();
		_path = reader.ReadString();
	}

	public SoundPoint(float time, string path)
	{
		_time = time;
		_path = path;
	}

	public float GetTime()
	{
		return _time;
	}

	public string GetPath()
	{
		return _path;
	}

	public int Size()
	{
		return 6 + MessageWriter.GetSize(_path);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteFloat(_time);
		writer.WriteString(_path);
	}
}
