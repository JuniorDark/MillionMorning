using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.Level.LevelInfo;

public class MilMo_WorldInfoData
{
	public string World { get; private set; }

	public MilMo_LocString WorldDisplayName { get; private set; }

	public string ScrollMode { get; private set; }

	public Vector2 ViewSize { get; private set; }

	public bool FixedPosition { get; private set; }

	public string WorldMapMusic { get; private set; }

	public Color BackgroundColor { get; private set; }

	public Vector2 LoadingPanePosition { get; private set; }

	public bool VisibleInGUILists { get; private set; }

	public MilMo_WorldInfoData(bool visibleInGuiLists)
	{
		ScrollMode = "NONE";
		WorldMapMusic = "";
		LoadingPanePosition = new Vector2(-67f, -84f);
		VisibleInGUILists = visibleInGuiLists;
	}

	public bool Read(MilMo_SFFile file)
	{
		while (file.NextRow())
		{
			if (file.IsNext("</WORLDMAP>"))
			{
				return true;
			}
			if (file.IsNext("World"))
			{
				World = file.GetString();
				continue;
			}
			if (file.IsNext("DisplayName"))
			{
				WorldDisplayName = MilMo_Localization.GetLocString(file.GetString());
				continue;
			}
			if (file.IsNext("ScrollMode"))
			{
				ScrollMode = file.GetString();
				continue;
			}
			if (file.IsNext("ViewSize"))
			{
				ViewSize = file.GetVector2();
				continue;
			}
			if (file.IsNext("FixedPosition"))
			{
				FixedPosition = true;
				continue;
			}
			if (file.IsNext("Music"))
			{
				WorldMapMusic = file.GetString();
				continue;
			}
			if (file.IsNext("BackgroundColor"))
			{
				BackgroundColor = file.GetColor();
				continue;
			}
			if (file.IsNext("LoadingPanePos"))
			{
				LoadingPanePosition = file.GetVector2();
				continue;
			}
			Debug.LogWarning($"Got unknown command in level travel info at line {file.GetLineNumber()} in file {file.Path}");
			return false;
		}
		Debug.LogWarning($"Failed to load level travel information in {file.Path} on line {file.GetLineNumber()}");
		return false;
	}
}
