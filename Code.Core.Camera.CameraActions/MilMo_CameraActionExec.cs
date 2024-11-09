using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionExec : MilMo_CameraAction
{
	private MilMo_SFFile _file;

	private readonly MilMo_EventSystem.MilMo_Callback _fileLoadedCallback;

	public MilMo_CameraActionExec(float time, MilMo_EventSystem.MilMo_Callback fileLoadedCallback)
		: base(time)
	{
		_fileLoadedCallback = fileLoadedCallback;
	}

	public override void Read(MilMo_SFFile file)
	{
		string @string = file.GetString();
		SetScript(@string);
	}

	public void SetScript(string path)
	{
		path = "Content/CameraScripts/" + path;
		MilMo_SimpleFormat.AsyncLoad(path, delegate(MilMo_SFFile scriptFile)
		{
			if (scriptFile == null)
			{
				Debug.LogWarning("File " + path + " not found.");
				if (_fileLoadedCallback != null)
				{
					_fileLoadedCallback();
				}
			}
			else
			{
				scriptFile.NextRow();
				if (scriptFile.PeekIsNext("Movie") || scriptFile.PeekIsNext("CameraSettings"))
				{
					_file = scriptFile;
					if (_fileLoadedCallback != null)
					{
						_fileLoadedCallback();
					}
				}
				else
				{
					if (_fileLoadedCallback != null)
					{
						_fileLoadedCallback();
					}
					Debug.LogWarning("Unknown camera script type '" + scriptFile.GetString());
				}
			}
		});
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		if (_file != null)
		{
			_file.Reset();
			_file.NextRow();
			cameraController.ExecuteScript(_file);
			_file.Reset();
		}
	}
}
