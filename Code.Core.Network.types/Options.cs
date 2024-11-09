namespace Code.Core.Network.types;

public class Options
{
	private readonly short _resolutionWidth;

	private readonly short _resolutionHeight;

	private readonly sbyte _fullscreen;

	private readonly sbyte _gfxSettings;

	private readonly float _musicVolume;

	private readonly sbyte _playEmotesOnChat;

	private readonly sbyte _showTutorials;

	private readonly sbyte _controlsMode;

	private readonly float _cameraSens;

	private readonly float _soundVolume;

	public Options(MessageReader reader)
	{
		_resolutionWidth = reader.ReadInt16();
		_resolutionHeight = reader.ReadInt16();
		_fullscreen = reader.ReadInt8();
		_gfxSettings = reader.ReadInt8();
		_musicVolume = reader.ReadFloat();
		_playEmotesOnChat = reader.ReadInt8();
		_showTutorials = reader.ReadInt8();
		_controlsMode = reader.ReadInt8();
		_cameraSens = reader.ReadFloat();
		_soundVolume = reader.ReadFloat();
	}

	public Options(short resolutionWidth, short resolutionHeight, sbyte fullscreen, sbyte gfxSettings, float musicVolume, sbyte playEmotesOnChat, sbyte showTutorials, sbyte controlsMode, float cameraSens, float soundVolume)
	{
		_resolutionWidth = resolutionWidth;
		_resolutionHeight = resolutionHeight;
		_fullscreen = fullscreen;
		_gfxSettings = gfxSettings;
		_musicVolume = musicVolume;
		_playEmotesOnChat = playEmotesOnChat;
		_showTutorials = showTutorials;
		_controlsMode = controlsMode;
		_cameraSens = cameraSens;
		_soundVolume = soundVolume;
	}

	public short GetResolutionWidth()
	{
		return _resolutionWidth;
	}

	public short GetResolutionHeight()
	{
		return _resolutionHeight;
	}

	public sbyte GetFullscreen()
	{
		return _fullscreen;
	}

	public sbyte GetGFXSettings()
	{
		return _gfxSettings;
	}

	public float GetMusicVolume()
	{
		return _musicVolume;
	}

	public float GetSoundVolume()
	{
		return _soundVolume;
	}

	public float GetCameraSens()
	{
		return _cameraSens;
	}

	public sbyte GetPlayEmotesOnChat()
	{
		return _playEmotesOnChat;
	}

	public sbyte GetShowTutorials()
	{
		return _showTutorials;
	}

	public sbyte GetControlsMode()
	{
		return _controlsMode;
	}

	public int Size()
	{
		return 21;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt16(_resolutionWidth);
		writer.WriteInt16(_resolutionHeight);
		writer.WriteInt8(_fullscreen);
		writer.WriteInt8(_gfxSettings);
		writer.WriteFloat(_musicVolume);
		writer.WriteInt8(_playEmotesOnChat);
		writer.WriteInt8(_showTutorials);
		writer.WriteInt8(_controlsMode);
		writer.WriteFloat(_cameraSens);
		writer.WriteFloat(_soundVolume);
	}
}
