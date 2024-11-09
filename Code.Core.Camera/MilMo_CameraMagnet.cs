using Code.Core.Collision;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_CameraMagnet
{
	private readonly MilMo_Volume _triggerVolume;

	private readonly GameObject _gameObject;

	public Vector3 CameraTarget { get; private set; }

	private MilMo_CameraMagnet(Vector3 cameraTarget, MilMo_Volume triggerVolume, GameObject gameObject)
	{
		CameraTarget = cameraTarget;
		_triggerVolume = triggerVolume;
		_gameObject = gameObject;
	}

	public bool IsInside(Vector3 position)
	{
		if (_triggerVolume != null)
		{
			return _triggerVolume.IsInside(position);
		}
		return false;
	}

	public void Destroy()
	{
		MilMo_Global.Destroy(_gameObject);
	}

	public static MilMo_CameraMagnet CreateFromFile(MilMo_SFFile file)
	{
		bool flag = false;
		bool flag2 = false;
		Vector3 position = Vector3.zero;
		Vector3 euler = Vector3.zero;
		Vector3 cameraTarget = Vector3.zero;
		MilMo_VolumeTemplate milMo_VolumeTemplate = null;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("TriggerPosition"))
			{
				position = file.GetVector3();
				flag = true;
			}
			else if (file.IsNext("TriggerRotation"))
			{
				euler = file.GetVector3();
			}
			else if (file.IsNext("TriggerVolume"))
			{
				milMo_VolumeTemplate = MilMo_VolumeTemplate.Create(file);
			}
			else if (file.IsNext("CameraTarget"))
			{
				cameraTarget = file.GetVector3();
				flag2 = true;
			}
			else
			{
				file.NextToken();
			}
		}
		if (!flag || !flag2 || milMo_VolumeTemplate == null)
		{
			Debug.LogWarning("Failed to read camera magnet from file " + file.Path + " at line " + file.GetLineNumber() + ". " + ((!flag) ? "TriggerPosition is missing. " : "") + ((!flag2) ? "CameraTarget is missing. " : "") + ((milMo_VolumeTemplate == null) ? "TriggerVolume is missing or invalid. " : ""));
			return null;
		}
		GameObject gameObject = new GameObject("Camera Magnet");
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.Euler(euler);
		MilMo_Volume triggerVolume = milMo_VolumeTemplate.Instantiate(gameObject.transform);
		return new MilMo_CameraMagnet(cameraTarget, triggerVolume, gameObject);
	}
}
