using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Emote;

public class MilMo_FaceMover
{
	private Vector2 _drag = new Vector2(0.95f, 0.95f);

	private Vector2 _pull = new Vector2(0.05f, 0.05f);

	private static readonly Dictionary<string, MilMo_FaceMover> FaceMovers = new Dictionary<string, MilMo_FaceMover>();

	private string Name { get; set; }

	public Vector2 Drag => _drag;

	public Vector2 Pull => _pull;

	public MilMo_FaceMover()
	{
		Name = "default";
	}

	private MilMo_FaceMover(MilMo_FaceMover faceMover)
	{
		Name = "default";
		if (faceMover != null)
		{
			Name = faceMover.Name;
			_drag = faceMover._drag;
			_pull = faceMover._pull;
		}
	}

	public static void LoadFaceMovers(string path)
	{
		FaceMovers.Clear();
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal(path);
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load face movers from " + path);
			return;
		}
		while (milMo_SFFile.NextRow())
		{
			MilMo_FaceMover milMo_FaceMover = new MilMo_FaceMover();
			if (!milMo_FaceMover.Load(milMo_SFFile))
			{
				Debug.LogWarning("Failed to load face mover at line " + milMo_SFFile.GetLineNumber());
			}
			else
			{
				FaceMovers[milMo_FaceMover.Name] = milMo_FaceMover;
			}
		}
	}

	public static MilMo_FaceMover GetFaceMover(string name)
	{
		FaceMovers.TryGetValue(name, out var value);
		return new MilMo_FaceMover(value);
	}

	private bool Load(MilMo_SFFile file)
	{
		if (!file.HasMoreTokens())
		{
			Debug.LogWarning("Bad face mover. Missing name.");
			return false;
		}
		Name = file.GetString();
		if (!file.HasMoreTokens())
		{
			return true;
		}
		_drag.x = file.GetFloat();
		if (!file.HasMoreTokens())
		{
			return true;
		}
		_drag.y = file.GetFloat();
		if (!file.HasMoreTokens())
		{
			return true;
		}
		_pull.x = file.GetFloat();
		if (!file.HasMoreTokens())
		{
			return true;
		}
		_pull.y = file.GetFloat();
		return true;
	}
}
