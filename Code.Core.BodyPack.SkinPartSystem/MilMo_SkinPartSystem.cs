using System;
using System.Collections.Generic;
using Code.Core.Config;
using UnityEngine;

namespace Code.Core.BodyPack.SkinPartSystem;

public static class MilMo_SkinPartSystem
{
	private static readonly bool DevMode = MilMo_Config.Instance.IsTrue("Debug.SkinPartSystem", defaultValue: false);

	private static Dictionary<string, MilMo_SkinPart> _theBoyMouths = new Dictionary<string, MilMo_SkinPart>(StringComparer.InvariantCultureIgnoreCase);

	private static Dictionary<string, MilMo_SkinPart> _theBoyEyes = new Dictionary<string, MilMo_SkinPart>(StringComparer.InvariantCultureIgnoreCase);

	private static Dictionary<string, MilMo_SkinPart> _theBoyEyeBrows = new Dictionary<string, MilMo_SkinPart>(StringComparer.InvariantCultureIgnoreCase);

	private static Dictionary<string, MilMo_SkinPart> _theGirlMouths = new Dictionary<string, MilMo_SkinPart>(StringComparer.InvariantCultureIgnoreCase);

	private static Dictionary<string, MilMo_SkinPart> _theGirlEyes = new Dictionary<string, MilMo_SkinPart>(StringComparer.InvariantCultureIgnoreCase);

	private static Dictionary<string, MilMo_SkinPart> _theGirlEyeBrows = new Dictionary<string, MilMo_SkinPart>(StringComparer.InvariantCultureIgnoreCase);

	private static MilMo_SkinPart _theMaleMainPart;

	private static MilMo_SkinPart _theFemaleMainPart;

	private static MilMo_SkinPart _theEyesMainPart;

	private static MilMo_SkinPart _theBoyTeethPart;

	private static MilMo_SkinPart _theGirlTeethPart;

	private static readonly Rect RegionMouth = new Rect(0f, 0.5f, 0.5f, 0.5f);

	private static readonly Rect RegionEyes = new Rect(0.5f, 0f, 45f / 128f, 0.5f);

	private static readonly Rect RegionEyeBrows = new Rect(0.39453125f, 0.5f, 0.10546875f, 0.5f);

	private static readonly Rect RegionMain = new Rect(0f, 0f, 0.5f, 0.5f);

	private static readonly Rect RegionTeeth = new Rect(0.5f, 57f / 128f, 45f / 128f, 7f / 128f);

	private static readonly Rect RegionEyesMain = new Rect(109f / 128f, 0f, 19f / 128f, 0.5f);

	private const int NR_OF_MONTHS = 6;

	private const int NR_OF_EYES = 6;

	private const int NR_OF_EYE_BROWS = 4;

	private static int _boyMouths;

	private static int _girlMouths;

	private static int _boyEyes;

	private static int _girlEyes;

	private static int _boyEyeBrows;

	private static int _girlEyeBrows;

	public static bool BoyDone
	{
		get
		{
			if (_boyMouths == 6 && _boyEyes == 6)
			{
				return _boyEyeBrows == 4;
			}
			return false;
		}
	}

	public static bool GirlDone
	{
		get
		{
			if (_girlMouths == 6 && _girlEyes == 6)
			{
				return _girlEyeBrows == 4;
			}
			return false;
		}
	}

	public static void CreateForBoy()
	{
		_theBoyMouths.Clear();
		_theBoyEyes.Clear();
		_theBoyEyeBrows.Clear();
		for (int i = 0; i < 6; i++)
		{
			MilMo_SkinPart milMo_SkinPart = new MilMo_SkinPart(Resources.Load("AvatarContent/Boy/Mouths/BoyMouth0" + (i + 1)) as Texture2D, RegionMouth);
			_theBoyMouths.Add(milMo_SkinPart.Name, milMo_SkinPart);
			_boyMouths++;
		}
		for (int j = 0; j < 6; j++)
		{
			MilMo_SkinPart milMo_SkinPart2 = new MilMo_SkinPart(Resources.Load("AvatarContent/Boy/Eyes/BoyEyes0" + (j + 1)) as Texture2D, RegionEyes);
			_theBoyEyes.Add(milMo_SkinPart2.Name, milMo_SkinPart2);
			_boyEyes++;
		}
		for (int k = 0; k < 4; k++)
		{
			MilMo_SkinPart milMo_SkinPart3 = new MilMo_SkinPart(Resources.Load("AvatarContent/Boy/EyeBrows/BoyEyeBrows0" + (k + 1)) as Texture2D, RegionEyeBrows);
			_theBoyEyeBrows.Add(milMo_SkinPart3.Name, milMo_SkinPart3);
			_boyEyeBrows++;
		}
		if (DevMode)
		{
			Debug.Log("MilMoSkinPartSystem: Boy skin part system created with " + _theBoyMouths.Count + " mouths, " + _theBoyEyes.Count + " eyes and " + _theBoyEyeBrows.Count + " eyebrows.");
		}
	}

	public static void CreateForGirl()
	{
		_theGirlMouths.Clear();
		_theGirlEyes.Clear();
		_theGirlEyeBrows.Clear();
		for (int i = 0; i < 6; i++)
		{
			MilMo_SkinPart milMo_SkinPart = new MilMo_SkinPart(Resources.Load("AvatarContent/Girl/Mouths/GirlMouth0" + (i + 1)) as Texture2D, RegionMouth);
			_theGirlMouths.Add(milMo_SkinPart.Name, milMo_SkinPart);
			_girlMouths++;
		}
		for (int j = 0; j < 6; j++)
		{
			MilMo_SkinPart milMo_SkinPart2 = new MilMo_SkinPart(Resources.Load("AvatarContent/Girl/Eyes/GirlEyes0" + (j + 1)) as Texture2D, RegionEyes);
			_theGirlEyes.Add(milMo_SkinPart2.Name, milMo_SkinPart2);
			_girlEyes++;
		}
		for (int k = 0; k < 4; k++)
		{
			MilMo_SkinPart milMo_SkinPart3 = new MilMo_SkinPart(Resources.Load("AvatarContent/Girl/EyeBrows/GirlEyeBrows0" + (k + 1)) as Texture2D, RegionEyeBrows);
			_theGirlEyeBrows.Add(milMo_SkinPart3.Name, milMo_SkinPart3);
			_girlEyeBrows++;
		}
		if (DevMode)
		{
			Debug.Log("MilMoSkinPartSystem: Girl skin part system created with " + _theGirlMouths.Count + " mouths, " + _theGirlEyes.Count + " eyes and " + _theGirlEyeBrows.Count + " eyebrows.");
		}
	}

	public static void CreateBoyMainPart(Texture2D texture)
	{
		_theMaleMainPart = new MilMo_SkinPart(texture, RegionMain);
		if (DevMode)
		{
			Debug.Log("MilMoSkinPartSystem: Boy main part created");
		}
	}

	public static void CreateGirlMainPart(Texture2D texture)
	{
		_theFemaleMainPart = new MilMo_SkinPart(texture, RegionMain);
		if (DevMode)
		{
			Debug.Log("MilMoSkinPartSystem: Girl main part created");
		}
	}

	public static void CreateBoyTeeth(Texture2D texture)
	{
		_theBoyTeethPart = new MilMo_SkinPart(texture, RegionTeeth);
		if (DevMode)
		{
			Debug.Log("MilMoSkinPartSystem: Boy teeth created");
		}
	}

	public static void CreateGirlTeeth(Texture2D texture)
	{
		_theGirlTeethPart = new MilMo_SkinPart(texture, RegionTeeth);
		if (DevMode)
		{
			Debug.Log("MilMoSkinPartSystem: Girl teeth created");
		}
	}

	public static void CreateEyes(Texture2D texture)
	{
		_theEyesMainPart = new MilMo_SkinPart(texture, RegionEyesMain);
		if (DevMode)
		{
			Debug.Log("MilMoSkinPartSystem: Eyes created");
		}
	}

	public static MilMo_SkinPart GetMouth(string mouthName, bool male)
	{
		if (!male)
		{
			return GetPart(mouthName, ref _theGirlMouths);
		}
		return GetPart(mouthName, ref _theBoyMouths);
	}

	public static MilMo_SkinPart GetEyes(string eyesName, bool male)
	{
		if (!male)
		{
			return GetPart(eyesName, ref _theGirlEyes);
		}
		return GetPart(eyesName, ref _theBoyEyes);
	}

	public static MilMo_SkinPart GetEyeBrows(string eyeBrowsName, bool male)
	{
		if (!male)
		{
			return GetPart(eyeBrowsName, ref _theGirlEyeBrows);
		}
		return GetPart(eyeBrowsName, ref _theBoyEyeBrows);
	}

	public static MilMo_SkinPart GetMainPart(bool male)
	{
		if (!male)
		{
			return _theFemaleMainPart;
		}
		return _theMaleMainPart;
	}

	public static MilMo_SkinPart GetMainEyes()
	{
		return _theEyesMainPart;
	}

	public static MilMo_SkinPart GetTeeth(bool male)
	{
		if (!male)
		{
			return _theGirlTeethPart;
		}
		return _theBoyTeethPart;
	}

	private static MilMo_SkinPart GetPart(string partName, ref Dictionary<string, MilMo_SkinPart> parts)
	{
		parts.TryGetValue(partName, out var value);
		return value;
	}
}
