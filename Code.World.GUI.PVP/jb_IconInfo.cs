using System;
using System.Collections.Generic;

namespace Code.World.GUI.PVP;

public class jb_IconInfo
{
	public string Path { get; private set; }

	public int Width { get; private set; }

	public int Height { get; private set; }

	public int OffsetX { get; private set; }

	public int OffsetY { get; private set; }

	public jb_IconInfo(string path, int width, int height, int offsetX, int offsetY)
	{
		Path = path;
		Width = width;
		Height = height;
		OffsetX = offsetX;
		OffsetY = offsetY;
	}

	public static ICollection<jb_IconInfo> GetIconContainers(MilMo_MatchMode matchMode)
	{
		List<jb_IconInfo> list = new List<jb_IconInfo>();
		switch (matchMode)
		{
		case MilMo_MatchMode.DEATH_MATCH:
			list.Add(new jb_IconInfo("Content/Bodypacks/Batch01/Generic/Icons/IconDeathMatch", 150, 80, 0, 0));
			break;
		case MilMo_MatchMode.CAPTURE_THE_FLAG:
			list.Add(new jb_IconInfo("Content/Bodypacks/Batch01/Generic/Icons/IconFlag", 60, 60, 0, 0));
			list.Add(new jb_IconInfo("Content/Bodypacks/Batch01/Generic/Icons/IconArrowForward", 35, 35, 0, -10));
			list.Add(new jb_IconInfo("Content/Bodypacks/Batch01/Generic/Icons/IconPVPCircleWhite", 100, 100, 0, 15));
			break;
		case MilMo_MatchMode.KING_OF_THE_HILL:
			list.Add(new jb_IconInfo("Content/Bodypacks/Batch01/Generic/Icons/IconKingOfTheHill", 220, 120, 0, 20));
			break;
		case MilMo_MatchMode.BATTLE_ROYALE:
			list.Add(new jb_IconInfo("Content/Bodypacks/Batch01/Generic/Icons/IconBattleRoyale", 200, 100, 0, 0));
			break;
		default:
			throw new ArgumentOutOfRangeException("matchMode", matchMode, null);
		}
		return list;
	}
}
