using System.Collections.Generic;
using System.Threading.Tasks;
using Localization;
using UI.Sprites;
using UnityEngine;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.Modal;

[CreateAssetMenu(menuName = "Testing/Create ModalDialogueSO", fileName = "ModalDialogueSO", order = 0)]
public class ModalDialogueSO : DialogueSO
{
	[SerializeField]
	protected LocalizedStringWithArgument caption;

	[SerializeField]
	protected LocalizedStringWithArgument message;

	[SerializeField]
	protected UnityEvent onConfirm = new UnityEvent();

	[SerializeField]
	protected LocalizedStringWithArgument confirmLabel;

	[SerializeField]
	protected UnityEvent onCancel = new UnityEvent();

	[SerializeField]
	protected LocalizedStringWithArgument cancelLabel;

	[SerializeField]
	protected UnityEvent onAlternate = new UnityEvent();

	[SerializeField]
	protected LocalizedStringWithArgument alternateLabel;

	[SerializeField]
	protected Sprite icon;

	[SerializeField]
	protected int lifetime;

	private bool _hasAlternate;

	private bool _hasCancel;

	private bool _confirmIsDefault;

	private bool _cancelIsDefault;

	private bool _alternateIsDefault;

	private IHaveSprite _spriteReference;

	public override string GetAddressableKey()
	{
		return "ModalDialogueWindow";
	}

	public override int GetPriority()
	{
		return 9;
	}

	public virtual void Init(ModalMessageData modalMessageData)
	{
		UnityAction unityAction = modalMessageData.GetOnConfirm();
		UnityAction unityAction2 = modalMessageData.GetOnCancel();
		UnityAction unityAction3 = modalMessageData.GetOnAlternate();
		_confirmIsDefault = modalMessageData.IsConfirmDefault();
		_cancelIsDefault = modalMessageData.IsCancelDefault();
		_alternateIsDefault = modalMessageData.IsAlternateDefault();
		caption = modalMessageData.GetCaption();
		message = modalMessageData.GetMessage();
		if (unityAction != null)
		{
			onConfirm.AddListener(unityAction);
		}
		confirmLabel = modalMessageData.GetConfirmLabel();
		if (unityAction2 != null)
		{
			onCancel.AddListener(unityAction2);
		}
		cancelLabel = modalMessageData.GetCancelLabel();
		if (unityAction3 != null)
		{
			onAlternate.AddListener(unityAction3);
		}
		alternateLabel = modalMessageData.GetAlternateLabel();
		_spriteReference = modalMessageData.GetSpriteReference();
		lifetime = modalMessageData.GetLifetime();
		_hasAlternate = unityAction3 != null || alternateLabel != null;
		_hasCancel = unityAction2 != null || cancelLabel != null;
	}

	public string GetCaption()
	{
		return caption?.GetMessage();
	}

	public string GetMessage(params object[] args)
	{
		if (args.Length != 0)
		{
			message?.SetFormatArgs(args);
		}
		return message?.GetMessage();
	}

	public int GetLifetime()
	{
		return lifetime;
	}

	public async Task<Sprite> GetIcon()
	{
		if (_spriteReference == null)
		{
			return null;
		}
		return await _spriteReference.GetSpriteAsync();
	}

	public void Confirm()
	{
		onConfirm?.Invoke();
		DialogueWindow.Close();
	}

	public void Alternate()
	{
		onAlternate?.Invoke();
		DialogueWindow.Close();
	}

	public void Cancel()
	{
		onCancel?.Invoke();
		DialogueWindow.Close();
	}

	public override List<DialogueButtonInfo> GetActions()
	{
		List<DialogueButtonInfo> list = new List<DialogueButtonInfo>
		{
			new DialogueButtonInfo(Confirm, confirmLabel ?? new LocalizedStringWithArgument("Generic_OK"), _confirmIsDefault)
		};
		if (_hasAlternate)
		{
			if (alternateLabel == null)
			{
				Debug.LogWarning($"ModalDialogueSO: '{caption}' is missing alternate label!");
			}
			list.Add(new DialogueButtonInfo(Alternate, alternateLabel ?? new LocalizedStringWithArgument("Alternate"), _alternateIsDefault));
		}
		if (_hasCancel)
		{
			list.Add(new DialogueButtonInfo(Cancel, cancelLabel ?? new LocalizedStringWithArgument("Generic_Cancel"), _cancelIsDefault));
		}
		return list;
	}
}
