using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionSet : MilMo_CameraAction
{
	private string _setting;

	private string[] _value;

	public MilMo_CameraActionSet(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
		_setting = file.GetString();
		List<string> list = new List<string>();
		while (file.HasMoreTokens())
		{
			list.Add(file.GetString());
		}
		_value = new string[list.Count];
		for (int i = 0; i < _value.Length; i++)
		{
			_value[i] = list[i];
		}
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		try
		{
			if (!cameraController.ApplySetting(_setting, _value))
			{
				Debug.LogWarning("Unknown setting or invalid value when executing 'Set' camera action at " + base.Time);
			}
		}
		catch (FormatException)
		{
			Debug.LogWarning("Failed to parse value when executing 'Set' camera action at " + base.Time + ": Invalid number format.");
		}
	}
}
