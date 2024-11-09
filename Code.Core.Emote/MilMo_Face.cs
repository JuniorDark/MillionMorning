using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Emote;

public class MilMo_Face
{
	private static readonly Dictionary<string, MilMo_Face> Faces = new Dictionary<string, MilMo_Face>();

	private static readonly Dictionary<string, MilMo_Face> Moods = new Dictionary<string, MilMo_Face>();

	public List<MilMo_Emote.FrameAction> Actions { get; private set; }

	public static void LoadFaces(string path)
	{
		Faces.Clear();
		TextAsset[] array = MilMo_ResourceManager.Instance.LoadAllLocal(path);
		foreach (TextAsset textAsset in array)
		{
			MilMo_Face value = new MilMo_Face(MilMo_SimpleFormat.LoadFromString(textAsset.text));
			Faces[textAsset.name] = value;
		}
	}

	public static void LoadMoods(string path)
	{
		Moods.Clear();
		TextAsset[] array = MilMo_ResourceManager.Instance.LoadAllLocal(path);
		foreach (TextAsset textAsset in array)
		{
			MilMo_Face value = new MilMo_Face(MilMo_SimpleFormat.LoadFromString(textAsset.text));
			Moods[textAsset.name] = value;
		}
	}

	public static MilMo_Face GetFace(string name)
	{
		if (name == null)
		{
			return null;
		}
		if (!Faces.ContainsKey(name))
		{
			return null;
		}
		return Faces[name];
	}

	public static MilMo_Face GetMood(string name)
	{
		if (name == null)
		{
			return null;
		}
		if (!Moods.ContainsKey(name))
		{
			return null;
		}
		return Moods[name];
	}

	private MilMo_Face(MilMo_SFFile face)
	{
		Actions = new List<MilMo_Emote.FrameAction>();
		while (face.NextRow())
		{
			float frameTime = 0f;
			if (face.PeekIsNext("At"))
			{
				face.NextToken();
				try
				{
					frameTime = face.GetFloat();
				}
				catch (Exception)
				{
					Debug.LogWarning("Failed to load action in face " + face.Name + " 'At' has now (or invalid) time at line " + face.GetLineNumber());
					frameTime = 0f;
				}
			}
			switch (face.GetString())
			{
			case "SnapUV":
			{
				MilMo_EmoteAction milMo_EmoteAction8 = new MilMo_EmoteActionSnapUV();
				milMo_EmoteAction8.Read(face);
				Actions.Add(new MilMo_Emote.FrameAction(milMo_EmoteAction8, frameTime));
				break;
			}
			case "SnapU":
			{
				MilMo_EmoteAction milMo_EmoteAction7 = new MilMo_EmoteActionSnapU();
				milMo_EmoteAction7.Read(face);
				Actions.Add(new MilMo_Emote.FrameAction(milMo_EmoteAction7, frameTime));
				break;
			}
			case "SnapV":
			{
				MilMo_EmoteAction milMo_EmoteAction6 = new MilMo_EmoteActionSnapV();
				milMo_EmoteAction6.Read(face);
				Actions.Add(new MilMo_Emote.FrameAction(milMo_EmoteAction6, frameTime));
				break;
			}
			case "GotoUV":
			{
				MilMo_EmoteAction milMo_EmoteAction5 = new MilMo_EmoteActionGotoUV();
				milMo_EmoteAction5.Read(face);
				Actions.Add(new MilMo_Emote.FrameAction(milMo_EmoteAction5, frameTime));
				break;
			}
			case "Rotate":
			{
				MilMo_EmoteAction milMo_EmoteAction4 = new MilMo_EmoteActionUVRotate();
				milMo_EmoteAction4.Read(face);
				Actions.Add(new MilMo_Emote.FrameAction(milMo_EmoteAction4, frameTime));
				break;
			}
			case "SnapRotate":
			{
				MilMo_EmoteAction milMo_EmoteAction3 = new MilMo_EmoteActionUVRotateSnap();
				milMo_EmoteAction3.Read(face);
				Actions.Add(new MilMo_Emote.FrameAction(milMo_EmoteAction3, frameTime));
				break;
			}
			case "GotoBone":
			{
				MilMo_EmoteAction milMo_EmoteAction2 = new MilMo_EmoteActionGotoBone();
				milMo_EmoteAction2.Read(face);
				Actions.Add(new MilMo_Emote.FrameAction(milMo_EmoteAction2, frameTime));
				break;
			}
			case "SetMood":
			{
				MilMo_EmoteAction milMo_EmoteAction = new MilMo_EmoteActionSetMood();
				milMo_EmoteAction.Read(face);
				Actions.Add(new MilMo_Emote.FrameAction(milMo_EmoteAction, frameTime));
				break;
			}
			}
		}
	}
}
