using System.Collections.Generic;

namespace Code.World.CharBuilder;

public interface ICharacterCustomStyles
{
	AvatarGender GetGender();

	float GetMaxHeight();

	float GetMinHeight();

	List<string> GetSkinColors();

	List<string> GetEyeColors();

	List<int> GetHairColors();

	List<string> GetMoods();

	List<IItem> GetHairStyles();

	List<string> GetEyeBrows();

	List<string> GetEyes();

	List<string> GetMouths();

	List<IItem> GetShirts();

	List<IItem> GetPants();

	List<IItem> GetShoes();

	AvatarSelection GetRandomAvatarSelection();
}
