using Code.Core.Items;

namespace Code.World.CharBuilder;

public interface IAvatarChange
{
	void ChangeHeight(float value);

	void ChangeSkinColor(string value);

	void ChangeHair(MilMo_Wearable value);

	void ChangeHairColor(int value);

	void ChangeEyeBrows(string value);

	void ChangeEyes(string value);

	void ChangeEyeColor(string value);

	void ChangeMouth(string value);

	void ChangeShirt(MilMo_Wearable value);

	void ChangeShirtColor(int value);

	void ChangePants(MilMo_Wearable value);

	void ChangePantsColor(int value);

	void ChangeShoes(MilMo_Wearable value);

	void ChangeShoesColor(int value, bool setPrimaryColor = true);

	void ChangeMood(string mood);
}
