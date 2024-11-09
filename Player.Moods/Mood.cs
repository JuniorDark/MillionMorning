using Localization;
using UnityEngine;

namespace Player.Moods;

[CreateAssetMenu(menuName = "Player/Create Mood", fileName = "Mood", order = 0)]
public class Mood : ScriptableObject
{
	[SerializeField]
	private string key;

	[SerializeField]
	private Sprite sprite;

	[SerializeField]
	private string nameLocKey;

	public string GetKey()
	{
		return key;
	}

	public string GetName()
	{
		return new LocalizedStringWithArgument(nameLocKey).GetMessage();
	}

	public Sprite GetSprite()
	{
		return sprite;
	}
}
