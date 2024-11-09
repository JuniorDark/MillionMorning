namespace Code.Core.Network.types;

public class HomeOfTheDay
{
	private readonly int _userId;

	private readonly string _avatarName;

	private readonly string _homeName;

	public HomeOfTheDay(MessageReader reader)
	{
		_userId = reader.ReadInt32();
		_avatarName = reader.ReadString();
		_homeName = reader.ReadString();
	}

	public HomeOfTheDay(int userId, string avatarName, string homeName)
	{
		_userId = userId;
		_avatarName = avatarName;
		_homeName = homeName;
	}

	public int GetUserId()
	{
		return _userId;
	}

	public string GetAvatarName()
	{
		return _avatarName;
	}

	public string GetHomeName()
	{
		return _homeName;
	}

	public int Size()
	{
		return 8 + MessageWriter.GetSize(_avatarName) + MessageWriter.GetSize(_homeName);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt32(_userId);
		writer.WriteString(_avatarName);
		writer.WriteString(_homeName);
	}
}
