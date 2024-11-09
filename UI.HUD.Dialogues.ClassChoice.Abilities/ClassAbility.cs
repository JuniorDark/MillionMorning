using UnityEngine;

namespace UI.HUD.Dialogues.ClassChoice.Abilities;

[CreateAssetMenu(fileName = "new classability", menuName = "Class/Ability", order = 0)]
public class ClassAbility : ScriptableObject
{
	[SerializeField]
	private ClassType type;

	[SerializeField]
	private string className;

	[SerializeField]
	private string description;

	[SerializeField]
	private int level;

	[SerializeField]
	private Sprite icon;

	public ClassType GetClassType()
	{
		return type;
	}

	public string GetClassName()
	{
		return className;
	}

	public string GetDescription()
	{
		return description;
	}

	public int GetLevel()
	{
		return level;
	}

	public Sprite GetIcon()
	{
		return icon;
	}
}
