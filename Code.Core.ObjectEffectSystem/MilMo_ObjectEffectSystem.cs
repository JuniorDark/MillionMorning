using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public static class MilMo_ObjectEffectSystem
{
	private static Dictionary<string, MilMo_ObjectEffectTemplate> _templates;

	public static bool Init()
	{
		_templates = new Dictionary<string, MilMo_ObjectEffectTemplate>();
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("ObjectEffects/ObjectEffects");
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load Object effects config file (ObjectEffects/ObjectEffects.txt)");
			return false;
		}
		while (milMo_SFFile.NextRow())
		{
			string @string = milMo_SFFile.GetString();
			MilMo_ObjectEffectTemplate milMo_ObjectEffectTemplate;
			switch (@string)
			{
			case "Fade":
				milMo_ObjectEffectTemplate = new MilMo_FadeEffectTemplate(milMo_SFFile);
				break;
			case "Scale":
				milMo_ObjectEffectTemplate = new MilMo_ScaleEffectTemplate(milMo_SFFile);
				break;
			case "Particle":
				milMo_ObjectEffectTemplate = new MilMo_ParticleObjectEffectTemplate(milMo_SFFile);
				break;
			case "Mover":
				milMo_ObjectEffectTemplate = new MilMo_MoverEffectTemplate(milMo_SFFile);
				break;
			case "Sound":
				milMo_ObjectEffectTemplate = new MilMo_SoundEffectTemplate(milMo_SFFile);
				break;
			case "Beach":
				milMo_ObjectEffectTemplate = new MilMo_BeachEffectTemplate(milMo_SFFile);
				break;
			case "Spin":
				milMo_ObjectEffectTemplate = new MilMo_SpinEffectTemplate(milMo_SFFile);
				break;
			case "Animation":
				milMo_ObjectEffectTemplate = new MilMo_AnimationEffectTemplate(milMo_SFFile);
				break;
			case "Blink":
				milMo_ObjectEffectTemplate = new MilMo_BlinkEffectTemplate(milMo_SFFile);
				break;
			case "Sink":
				milMo_ObjectEffectTemplate = new MilMo_SinkEffectTemplate(milMo_SFFile);
				break;
			case "AttachMesh":
				milMo_ObjectEffectTemplate = new MilMo_AttachMeshEffectTemplate(milMo_SFFile);
				break;
			case "Gib":
				milMo_ObjectEffectTemplate = new MilMo_GibEffectTemplate(milMo_SFFile);
				break;
			case "HardBlink":
				milMo_ObjectEffectTemplate = new MilMo_HardBlinkEffectTemplate(milMo_SFFile);
				break;
			case "ColorTint":
				milMo_ObjectEffectTemplate = new MilMo_ColorTintEffectTemplate(milMo_SFFile);
				break;
			case "Outline":
				milMo_ObjectEffectTemplate = new MilMo_OutlineEffectTemplate(milMo_SFFile);
				break;
			case "Transparent":
				milMo_ObjectEffectTemplate = new MilMo_TransparentEffectTemplate(milMo_SFFile);
				break;
			case "GemTransparency":
				milMo_ObjectEffectTemplate = new MilMo_GemTransparencyEffectTemplate(milMo_SFFile);
				break;
			case "Camera":
				milMo_ObjectEffectTemplate = new MilMo_CameraEffectTemplate(milMo_SFFile);
				break;
			default:
				Debug.LogWarning("Got Object effect template of unknown type ('" + @string + "')");
				continue;
			}
			try
			{
				_templates.Add(milMo_ObjectEffectTemplate.Name, milMo_ObjectEffectTemplate);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("Got non-unique effect name '" + milMo_ObjectEffectTemplate.Name + "' in object effects file");
			}
		}
		return true;
	}

	public static MilMo_ObjectEffect GetObjectEffect(GameObject gameObject, string effectName)
	{
		if (_templates == null || gameObject == null)
		{
			return null;
		}
		if (!_templates.TryGetValue(effectName, out var value))
		{
			Debug.LogWarning("Object effect template " + effectName + " could not be found.");
			return null;
		}
		return value.CreateObjectEffect(gameObject);
	}

	public static string Debug_ReloadObjectEffects(string[] args)
	{
		if (Init())
		{
			return _templates.Count + " object effects loaded";
		}
		return "Failed to reload object effects";
	}
}
