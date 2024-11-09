using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.BodyPack;
using Code.Core.Items;
using UnityEngine;

namespace Code.World.CharBuilder.MakeOverStudio;

public abstract class MilMo_CharBuilderData
{
	public readonly List<string> SkinColors = new List<string>();

	public readonly List<int> HairColors = new List<int>();

	public readonly List<string> EyeColors = new List<string>();

	public readonly List<string> BoyMouths = new List<string>();

	public readonly List<string> BoyEyes = new List<string>();

	public readonly List<string> BoyEyeBrows = new List<string>();

	public readonly List<string> BoyHairStyles = new List<string>();

	public readonly List<string> BoyShirts = new List<string>();

	public readonly List<string> BoyPants = new List<string>();

	public readonly List<string> BoyShoes = new List<string>();

	public readonly List<MilMo_Wearable> BoyHairStyleItems = new List<MilMo_Wearable>();

	public readonly List<MilMo_Wearable> BoyShirtItems = new List<MilMo_Wearable>();

	public readonly List<MilMo_Wearable> BoyPantsItems = new List<MilMo_Wearable>();

	public readonly List<MilMo_Wearable> BoyShoesItems = new List<MilMo_Wearable>();

	public readonly List<string> GirlMouths = new List<string>();

	public readonly List<string> GirlEyes = new List<string>();

	public readonly List<string> GirlEyeBrows = new List<string>();

	public readonly List<string> GirlHairStyles = new List<string>();

	public readonly List<string> GirlShirts = new List<string>();

	public readonly List<string> GirlPants = new List<string>();

	public readonly List<string> GirlShoes = new List<string>();

	public readonly List<MilMo_Wearable> GirlHairStyleItems = new List<MilMo_Wearable>();

	public readonly List<MilMo_Wearable> GirlShirtItems = new List<MilMo_Wearable>();

	public readonly List<MilMo_Wearable> GirlPantsItems = new List<MilMo_Wearable>();

	public readonly List<MilMo_Wearable> GirlShoesItems = new List<MilMo_Wearable>();

	public int GetSkinColorIndex(string skinColor)
	{
		for (int i = 0; i < SkinColors.Count; i++)
		{
			if (SkinColors[i] == skinColor)
			{
				return i;
			}
		}
		return 0;
	}

	public int GetEyeColorIndex(string eyeColor)
	{
		for (int i = 0; i < EyeColors.Count; i++)
		{
			if (EyeColors[i] == eyeColor)
			{
				return i;
			}
		}
		return 0;
	}

	public int GetHairColorIndex(string hairColor)
	{
		try
		{
			for (int i = 0; i < HairColors.Count; i++)
			{
				if (HairColors[i] == int.Parse(hairColor))
				{
					return i;
				}
			}
			return 0;
		}
		catch (FormatException)
		{
			Debug.LogWarning("Got format exception for hair color " + hairColor);
			return 0;
		}
	}

	public int GetEyesIndex(string eyes, int gender)
	{
		if (gender == 0)
		{
			for (int i = 0; i < BoyEyes.Count; i++)
			{
				if (BoyEyes[i] == eyes)
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < GirlEyes.Count; j++)
			{
				if (GirlEyes[j] == eyes)
				{
					return j;
				}
			}
		}
		return 0;
	}

	public int GetEyeBrowsIndex(string eyeBrows, int gender)
	{
		if (gender == 0)
		{
			for (int i = 0; i < BoyEyeBrows.Count; i++)
			{
				if (BoyEyeBrows[i] == eyeBrows)
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < GirlEyeBrows.Count; j++)
			{
				if (GirlEyeBrows[j] == eyeBrows)
				{
					return j;
				}
			}
		}
		return 0;
	}

	public int GetMouthIndex(string mouth, int gender)
	{
		if (gender == 0)
		{
			for (int i = 0; i < BoyMouths.Count; i++)
			{
				if (BoyMouths[i] == mouth)
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < GirlMouths.Count; j++)
			{
				if (GirlMouths[j] == mouth)
				{
					return j;
				}
			}
		}
		return 0;
	}

	public int GetHairIndex(string hair, int gender)
	{
		if (gender == 0)
		{
			for (int i = 0; i < BoyHairStyleItems.Count; i++)
			{
				if (BoyHairStyleItems[i].Template.Identifier == hair)
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < GirlHairStyleItems.Count; j++)
			{
				if (GirlHairStyleItems[j].Template.Identifier == hair)
				{
					return j;
				}
			}
		}
		return 0;
	}

	public MilMo_Wearable GetHairStyleByIdentifier(string identifier, int gender)
	{
		int hairIndex = GetHairIndex("BodyPack:" + identifier, gender);
		if (gender != 0)
		{
			return GirlHairStyleItems[hairIndex];
		}
		return BoyHairStyleItems[hairIndex];
	}

	public int GetShirtIndex(string shirt, int gender, bool defaultToFirstShirt = true)
	{
		if (gender == 0)
		{
			for (int i = 0; i < BoyShirtItems.Count; i++)
			{
				if (BoyShirtItems[i].Template.Identifier == shirt)
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < GirlShirtItems.Count; j++)
			{
				if (GirlShirtItems[j].Template.Identifier == shirt)
				{
					return j;
				}
			}
		}
		if (!defaultToFirstShirt)
		{
			return -1;
		}
		return 0;
	}

	public int GetShirtColorIndex(string shirt, string color, int gender)
	{
		try
		{
			if (gender == 0)
			{
				using IEnumerator<MilMo_Wearable> enumerator = BoyShirtItems.Where((MilMo_Wearable t) => t.Template.Identifier == shirt).GetEnumerator();
				if (enumerator.MoveNext())
				{
					MilMo_Wearable current = enumerator.Current;
					for (int i = 0; i < current.BodyPack.ColorGroups[0].ColorIndices.Count; i++)
					{
						if (current.BodyPack.ColorGroups[0].ColorIndices[i] == int.Parse(color))
						{
							return i;
						}
					}
					return 0;
				}
			}
			else
			{
				using IEnumerator<MilMo_Wearable> enumerator = GirlShirtItems.Where((MilMo_Wearable t) => t.Template.Identifier == shirt).GetEnumerator();
				if (enumerator.MoveNext())
				{
					MilMo_Wearable current2 = enumerator.Current;
					for (int j = 0; j < current2.BodyPack.ColorGroups[0].ColorIndices.Count; j++)
					{
						if (current2.BodyPack.ColorGroups[0].ColorIndices[j] == int.Parse(color))
						{
							return j;
						}
					}
					return 0;
				}
			}
			return 0;
		}
		catch (IndexOutOfRangeException)
		{
			Debug.LogWarning("Got index out of bounds when fetching index for shirts color (" + shirt + ", " + color + ", " + gender + ")");
			return 0;
		}
	}

	public MilMo_Wearable GetShirtByIdentifier(string identifier, int gender)
	{
		int shirtIndex = GetShirtIndex("BodyPack:" + identifier, gender);
		if (gender != 0)
		{
			return GirlShirtItems[shirtIndex];
		}
		return BoyShirtItems[shirtIndex];
	}

	public int GetPantsIndex(string pants, int gender, bool defaultToFirstPants = true)
	{
		if (gender == 0)
		{
			for (int i = 0; i < BoyPantsItems.Count; i++)
			{
				if (BoyPantsItems[i].Template.Identifier == pants)
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < GirlPantsItems.Count; j++)
			{
				if (GirlPantsItems[j].Template.Identifier == pants)
				{
					return j;
				}
			}
		}
		if (!defaultToFirstPants)
		{
			return -1;
		}
		return 0;
	}

	public int GetPantsColorIndex(string pants, string color, int gender)
	{
		try
		{
			if (gender == 0)
			{
				using IEnumerator<MilMo_Wearable> enumerator = BoyPantsItems.Where((MilMo_Wearable t) => t.Template.Identifier == pants).GetEnumerator();
				if (enumerator.MoveNext())
				{
					MilMo_Wearable current = enumerator.Current;
					for (int i = 0; i < current.BodyPack.ColorGroups[0].ColorIndices.Count; i++)
					{
						if (current.BodyPack.ColorGroups[0].ColorIndices[i] == int.Parse(color))
						{
							return i;
						}
					}
					return 0;
				}
			}
			else
			{
				using IEnumerator<MilMo_Wearable> enumerator = GirlPantsItems.Where((MilMo_Wearable t) => t.Template.Identifier == pants).GetEnumerator();
				if (enumerator.MoveNext())
				{
					MilMo_Wearable current2 = enumerator.Current;
					for (int j = 0; j < current2.BodyPack.ColorGroups[0].ColorIndices.Count; j++)
					{
						if (current2.BodyPack.ColorGroups[0].ColorIndices[j] == int.Parse(color))
						{
							return j;
						}
					}
					return 0;
				}
			}
			return 0;
		}
		catch (IndexOutOfRangeException)
		{
			Debug.LogWarning("Got index out of bounds when fetching index for pants color (" + pants + ", " + color + ", " + gender + ")");
			return 0;
		}
	}

	public MilMo_Wearable GetPantsByIdentifier(string identifier, int gender)
	{
		int pantsIndex = GetPantsIndex("BodyPack:" + identifier, gender);
		if (gender != 0)
		{
			return GirlPantsItems[pantsIndex];
		}
		return BoyPantsItems[pantsIndex];
	}

	public int GetShoesIndex(string shoes, int gender, bool defaultToFirstShoes = true)
	{
		if (gender == 0)
		{
			for (int i = 0; i < BoyShoesItems.Count; i++)
			{
				if (BoyShoesItems[i].Template.Identifier == shoes)
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < GirlShoesItems.Count; j++)
			{
				if (GirlShoesItems[j].Template.Identifier == shoes)
				{
					return j;
				}
			}
		}
		if (!defaultToFirstShoes)
		{
			return -1;
		}
		return 0;
	}

	public MilMo_Wearable GetShoesByIdentifier(string identifier, int gender)
	{
		int shoesIndex = GetShoesIndex("BodyPack:" + identifier, gender);
		if (gender != 0)
		{
			return GirlShoesItems[shoesIndex];
		}
		return BoyShoesItems[shoesIndex];
	}

	public int GetShoesFirstColorIndex(string shoes, string color, string colorGroupName, int gender)
	{
		try
		{
			if (gender == 0)
			{
				using IEnumerator<MilMo_Wearable> enumerator = BoyShoesItems.Where((MilMo_Wearable t) => t.Template.Identifier == shoes).GetEnumerator();
				if (enumerator.MoveNext())
				{
					foreach (ColorGroup item in enumerator.Current.BodyPack.ColorGroups.Where((ColorGroup g) => g.GroupName == colorGroupName))
					{
						for (int i = 0; i < item.ColorIndices.Count; i++)
						{
							if (item.ColorIndices[i] == int.Parse(color))
							{
								return i;
							}
						}
					}
					return 0;
				}
			}
			else
			{
				using IEnumerator<MilMo_Wearable> enumerator = GirlShoesItems.Where((MilMo_Wearable t) => t.Template.Identifier == shoes).GetEnumerator();
				if (enumerator.MoveNext())
				{
					foreach (ColorGroup item2 in enumerator.Current.BodyPack.ColorGroups.Where((ColorGroup g) => g.GroupName == colorGroupName))
					{
						for (int j = 0; j < item2.ColorIndices.Count; j++)
						{
							if (item2.ColorIndices[j] == int.Parse(color))
							{
								return j;
							}
						}
					}
					return 0;
				}
			}
			return 0;
		}
		catch (IndexOutOfRangeException)
		{
			Debug.LogWarning("Got index out of bounds when fetching index for shoes first color (" + shoes + ", " + color + ", " + gender + ")");
			return 0;
		}
	}

	public int GetShoesSecondColorIndex(string shoes, string color, string colorGroupName, int gender)
	{
		try
		{
			if (gender == 0)
			{
				using IEnumerator<MilMo_Wearable> enumerator = BoyShoesItems.Where((MilMo_Wearable t) => t.Template.Identifier == shoes).GetEnumerator();
				if (enumerator.MoveNext())
				{
					foreach (ColorGroup item in enumerator.Current.BodyPack.ColorGroups.Where((ColorGroup g) => g.GroupName == colorGroupName))
					{
						for (int i = 0; i < item.ColorIndices.Count; i++)
						{
							if (item.ColorIndices[i] == int.Parse(color))
							{
								return i;
							}
						}
					}
					return 0;
				}
			}
			else
			{
				using IEnumerator<MilMo_Wearable> enumerator = GirlShoesItems.Where((MilMo_Wearable t) => t.Template.Identifier == shoes).GetEnumerator();
				if (enumerator.MoveNext())
				{
					foreach (ColorGroup item2 in enumerator.Current.BodyPack.ColorGroups.Where((ColorGroup g) => g.GroupName == colorGroupName))
					{
						for (int j = 0; j < item2.ColorIndices.Count; j++)
						{
							if (item2.ColorIndices[j] == int.Parse(color))
							{
								return j;
							}
						}
					}
					return 0;
				}
			}
			return 0;
		}
		catch (IndexOutOfRangeException)
		{
			Debug.LogWarning("Got index out of bounds when fetching index for shoes second color (" + shoes + ", " + color + ", " + gender + ")");
			return 0;
		}
	}

	public abstract Task<bool> LoadDataAsync();
}
