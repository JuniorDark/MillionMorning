using System.Linq;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using Core.Audio.AudioData;
using TMPro;
using UI.FX;
using UI.HUD.Dialogues.ClassChoice.Abilities;
using UI.Sprites;
using UnityEngine;

namespace UI.HUD.Dialogues.ClassChoice;

public class ClassChoiceDialogueWindow : DialogueWindow
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text caption;

	[SerializeField]
	private ClassChoice dps;

	[SerializeField]
	private ClassChoice support;

	[SerializeField]
	private ClassChoice protection;

	[Header("Setup")]
	[SerializeField]
	private ClassAbility[] abilities;

	[Header("Sounds")]
	[SerializeField]
	protected UIAudioCueSO dialogueOpenSound;

	[SerializeField]
	protected UIAudioCueSO dialogueCloseSound;

	private UIAlphaFX _fader;

	private UIScaleFX _scaler;

	private ClassChoiceDialogueSO _classChoiceDialogueSO;

	private int _selectedPath;

	public override void Init(DialogueSO so)
	{
		_classChoiceDialogueSO = (ClassChoiceDialogueSO)so;
		if (_classChoiceDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type");
			return;
		}
		base.Init(so);
		caption.text = MilMo_Localization.GetLocString("ProfileWindow_10239").String;
		int level = _classChoiceDialogueSO.GetLevel();
		dps.Init(GetClassAbilitiesAtLevel(level, ClassType.Dps));
		support.Init(GetClassAbilitiesAtLevel(level, ClassType.Healer));
		protection.Init(GetClassAbilitiesAtLevel(level, ClassType.Tank));
	}

	private void Awake()
	{
		_fader = GetComponentInChildren<UIAlphaFX>();
		if (!_fader)
		{
			Debug.LogWarning(base.name + ": Unable to find UIAlphaFX");
			return;
		}
		_scaler = GetComponentInChildren<UIScaleFX>();
		if (!_scaler)
		{
			Debug.LogWarning(base.name + ": Unable to find UIScaleFX");
		}
	}

	private void Start()
	{
		MilMo_EventSystem.Instance.PostEvent("tutorial_ClassSelection", "");
	}

	public void UpdateSelection(int choice)
	{
		_selectedPath = choice;
	}

	public void Select()
	{
		if ((bool)_fader)
		{
			_fader.FadeOut();
		}
		if ((bool)_scaler)
		{
			_scaler.Shrink();
		}
		if (dialogueCloseSound != null)
		{
			dialogueCloseSound.PlayAudioCue();
		}
		base.Close();
		DialogueSpawner.SpawnYesNoModal("ProfileWindow_10239", "ProfileWindow_11383", new AddressableSpriteLoader("WarningIcon"), delegate
		{
			_classChoiceDialogueSO.TellServer(_selectedPath);
		}, null);
	}

	private ClassAbility[] GetClassAbilitiesAtLevel(int level, ClassType type)
	{
		return abilities.Where((ClassAbility ability) => ability.GetLevel() == level && ability.GetClassType() == type).ToArray();
	}
}
