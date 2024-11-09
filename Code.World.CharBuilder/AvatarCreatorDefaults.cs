using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.World.CharBuilder;

public abstract class AvatarCreatorDefaults : ICharacterCustomStyles
{
	private readonly List<string> _skinColors = new List<string> { "Skin0002", "Skin0003", "Skin0001", "Skin0004", "Skin0005", "Skin0006", "Skin0007", "Skin0008", "Skin0008", "Skin0008" };

	private readonly List<string> _eyeColors = new List<string> { "Amber", "Blue", "Grey", "GreenLight", "Green", "GreyYellow", "Gold", "Orange", "BrownLight", "Brown" };

	private readonly List<int> _hairColors = new List<int> { 63, 64, 65, 73, 75, 68, 67, 70, 71, 57 };

	private readonly List<string> _moods = new List<string> { "Normal", "Angry", "Happy", "Sad" };

	public abstract AvatarGender GetGender();

	public abstract float GetMaxHeight();

	public abstract float GetMinHeight();

	public List<string> GetSkinColors()
	{
		return _skinColors;
	}

	public List<string> GetEyeColors()
	{
		return _eyeColors;
	}

	public List<int> GetHairColors()
	{
		return _hairColors;
	}

	public List<string> GetMoods()
	{
		return _moods;
	}

	public abstract List<IItem> GetHairStyles();

	public IItem FindHairStyle(string bodypack)
	{
		return GetHairStyles().FirstOrDefault(delegate(IItem e)
		{
			string text = bodypack.Replace("Bodypacks.", "");
			return e.GetBodyPack() == text;
		});
	}

	public abstract List<string> GetEyeBrows();

	public abstract List<string> GetEyes();

	public abstract List<string> GetMouths();

	public abstract List<IItem> GetShirts();

	public IItem FindShirt(string bodypack)
	{
		return GetShirts().FirstOrDefault(delegate(IItem e)
		{
			string text = bodypack.Replace("Bodypacks.", "");
			return e.GetBodyPack() == text;
		});
	}

	public abstract List<IItem> GetPants();

	public IItem FindPants(string bodypack)
	{
		return GetPants().FirstOrDefault(delegate(IItem e)
		{
			string text = bodypack.Replace("Bodypacks.", "");
			return e.GetBodyPack() == text;
		});
	}

	public abstract List<IItem> GetShoes();

	public IItem FindShoe(string bodypack)
	{
		return GetShoes().FirstOrDefault(delegate(IItem e)
		{
			string text = bodypack.Replace("Bodypacks.", "");
			return e.GetBodyPack() == text;
		});
	}

	public AvatarSelection GetRandomAvatarSelection()
	{
		AvatarGender gender = GetGender();
		float between = GetBetween(GetMinHeight(), GetMaxHeight());
		string random = GetRandom(GetSkinColors());
		string random2 = GetRandom(GetEyeColors());
		int random3 = GetRandom(GetHairColors());
		string random4 = GetRandom(GetEyeBrows());
		string random5 = GetRandom(GetEyes());
		string random6 = GetRandom(GetMouths());
		IItem random7 = GetRandom(GetShirts());
		IItem random8 = GetRandom(GetPants());
		IItem random9 = GetRandom(GetShoes());
		IItem random10 = GetRandom(GetHairStyles());
		return new AvatarSelection((byte)gender, between, random5, random4, random6, random, random2, random3, random7, random8, random9, random10);
	}

	private static T GetRandom<T>(IList<T> list)
	{
		return list[Random.Range(0, list.Count - 1)];
	}

	private static float GetBetween(float a, float b)
	{
		return Random.Range(a, b);
	}
}
