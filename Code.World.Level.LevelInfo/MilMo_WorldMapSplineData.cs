using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.Level.LevelInfo;

public class MilMo_WorldMapSplineData
{
	private Vector2 _startPos = Vector2.zero;

	private Vector2 _midPos = Vector2.zero;

	private Vector2 _endPos = Vector2.zero;

	public string World { get; private set; }

	public string Level1 { get; private set; }

	public string Level2 { get; private set; }

	public int Points { get; private set; }

	public Vector2 StartPosRes(Vector2 res)
	{
		return new Vector2(_startPos.x * res.x, _startPos.y * res.y);
	}

	public Vector2 MidPosRes(Vector2 res)
	{
		return new Vector2(_midPos.x * res.x, _midPos.y * res.y);
	}

	public Vector2 EndPosRes(Vector2 res)
	{
		return new Vector2(_endPos.x * res.x, _endPos.y * res.y);
	}

	public bool Read(MilMo_SFFile file)
	{
		while (file.NextRow())
		{
			if (file.IsNext("</SPLINE>"))
			{
				if (Level1 != null)
				{
					return Level2 != null;
				}
				return false;
			}
			if (file.IsNext("World"))
			{
				World = file.GetString();
				continue;
			}
			if (file.IsNext("Level1"))
			{
				Level1 = file.GetString();
				continue;
			}
			if (file.IsNext("Level2"))
			{
				Level2 = file.GetString();
				continue;
			}
			if (file.IsNext("StartPos"))
			{
				_startPos = file.GetVector2();
				continue;
			}
			if (file.IsNext("MidPos"))
			{
				_midPos = file.GetVector2();
				continue;
			}
			if (file.IsNext("EndPos"))
			{
				_endPos = file.GetVector2();
				continue;
			}
			if (file.IsNext("Points"))
			{
				Points = file.GetInt() + 1;
				continue;
			}
			Debug.LogWarning($"Got unknown command in world map spline info at line {file.GetLineNumber()} in file {file.Path}");
			return false;
		}
		Debug.LogWarning($"Failed to load world map spline information in {file.Path} on line {file.GetLineNumber()}");
		return false;
	}
}
