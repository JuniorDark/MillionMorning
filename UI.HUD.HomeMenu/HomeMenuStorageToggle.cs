using System;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Sound;
using Code.World;
using Code.World.Home;
using Code.World.Player;
using Core.Utilities;
using UI.Elements.Slot;
using UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.HUD.HomeMenu;

public class HomeMenuStorageToggle : HomeMenuToggle, IPointerClickHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private SimpleTooltipTrigger storageTooltip;

	[SerializeField]
	private SimpleTooltipTrigger moveToStorageTooltip;

	[SerializeField]
	private Image moveToStorageArrow;

	public UnityEvent onMoveToStorage;

	private bool _holdingFurnitureModeOn;

	private int _arrowMoveAnimation;

	private int _arrowFadeAnimation;

	private CanvasGroup _target;

	private Vector3 _originalArrowPosition = Vector3.zero;

	private Canvas _canvas;

	protected override void Awake()
	{
		base.Awake();
		if (moveToStorageArrow == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing moveToStorageArrow");
			return;
		}
		_target = moveToStorageArrow.GetComponent<CanvasGroup>();
		if (!_target)
		{
			_target = moveToStorageArrow.gameObject.AddComponent<CanvasGroup>();
		}
		if (_target == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Could not create CanvasGroup for moveToStorageArrow");
		}
		else
		{
			_target.alpha = 0f;
		}
	}

	private void Start()
	{
		_canvas = Core.Utilities.UI.GetCanvas(base.gameObject);
		if (_canvas == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Could not find any Canvas");
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		OnEnterHoldFurnitureModeEvent(value: false);
	}

	public void OnPanelClosed(bool value)
	{
		if (!value && !(toggle == null) && !(toggle.targetGraphic == null))
		{
			if (_holdingFurnitureModeOn)
			{
				WasOn = false;
				return;
			}
			toggle.SetIsOnWithoutNotify(value: false);
			toggle.targetGraphic.enabled = true;
		}
	}

	public void OnEnterHoldFurnitureModeEvent(bool value)
	{
		if (!(toggle == null) && !(toggle.targetGraphic == null))
		{
			_holdingFurnitureModeOn = value;
			if (_holdingFurnitureModeOn)
			{
				toggle.SetIsOnWithoutNotify(value: true);
				toggle.targetGraphic.enabled = false;
				moveToStorageTooltip.active = true;
				storageTooltip.active = false;
			}
			else
			{
				toggle.SetIsOnWithoutNotify(WasOn);
				toggle.targetGraphic.enabled = !WasOn;
				moveToStorageTooltip.active = false;
				storageTooltip.active = true;
				HideArrow();
			}
			toggle.interactable = !_holdingFurnitureModeOn;
		}
	}

	private void Stash()
	{
		MilMo_EventSystem.Instance.PostEvent("move_furniture_to_storage", null);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (_holdingFurnitureModeOn)
		{
			Stash();
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (_holdingFurnitureModeOn)
		{
			ToggleMoveToStorageArrow(shouldEnable: true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (_holdingFurnitureModeOn)
		{
			ToggleMoveToStorageArrow(shouldEnable: false);
		}
	}

	private void ToggleMoveToStorageArrow(bool shouldEnable)
	{
		if (shouldEnable)
		{
			ShowArrow();
		}
		else
		{
			HideArrow();
		}
	}

	private void ShowArrow()
	{
		if (!(moveToStorageArrow == null))
		{
			if (_arrowFadeAnimation != 0)
			{
				LeanTween.cancel(_arrowFadeAnimation);
			}
			moveToStorageArrow.gameObject.SetActive(value: true);
			StartMovingArrow();
			_arrowFadeAnimation = LeanTween.alphaCanvas(_target, 1f, 1f).setEase(LeanTweenType.easeInSine).setOnComplete((Action)delegate
			{
				_arrowFadeAnimation = 0;
			})
				.id;
		}
	}

	private void HideArrow()
	{
		if (!(moveToStorageArrow == null))
		{
			if (_arrowFadeAnimation != 0)
			{
				LeanTween.cancel(_arrowFadeAnimation);
			}
			_arrowFadeAnimation = LeanTween.alphaCanvas(_target, 0f, 1f).setEase(LeanTweenType.easeInSine).setOnComplete((Action)delegate
			{
				_arrowFadeAnimation = 0;
				StopMovingArrow();
				moveToStorageArrow.gameObject.SetActive(value: false);
			})
				.id;
		}
	}

	private void StartMovingArrow()
	{
		if (_arrowMoveAnimation == 0)
		{
			if (_originalArrowPosition == Vector3.zero)
			{
				_originalArrowPosition = moveToStorageArrow.transform.position;
			}
			_arrowMoveAnimation = LeanTween.move(moveToStorageArrow.gameObject, base.gameObject.transform, 1f).setLoopPingPong().setEase(LeanTweenType.easeInOutSine)
				.id;
		}
	}

	private void StopMovingArrow()
	{
		if (_arrowMoveAnimation != 0)
		{
			moveToStorageArrow.transform.position = _originalArrowPosition;
			LeanTween.cancel(_arrowMoveAnimation);
			_arrowMoveAnimation = 0;
		}
	}

	public void MoveEntryToStorage(ISlotItemEntry entry)
	{
		if (_canvas == null)
		{
			return;
		}
		IEntryItem entryItem = entry?.GetItem();
		if (entryItem == null)
		{
			return;
		}
		Texture2D itemIcon = entryItem.GetItemIcon();
		if (!(itemIcon == null) && entryItem is MilMo_HomeFurniture { Position: var position })
		{
			GameObject clone = new GameObject("GoingIntoStorageClone");
			clone.transform.parent = _canvas.transform;
			MilMo_PlayerControllerHome milMo_PlayerControllerHome = MilMo_World.Instance.PlayerController as MilMo_PlayerControllerHome;
			Vector2 position2 = MilMo_Global.MainCamera.WorldToScreenPoint(position);
			if (milMo_PlayerControllerHome != null && milMo_PlayerControllerHome.FurnishingMenu != null)
			{
				position2 = milMo_PlayerControllerHome.FurnishingMenu.Pos;
				milMo_PlayerControllerHome.FurnishingMenu.Close();
			}
			Rect rect = new Rect(position2, new Vector2(80f, 80f));
			Sprite sprite = Sprite.Create(itemIcon, rect, Vector2.zero);
			Image image = clone.AddComponent<Image>();
			image.sprite = sprite;
			image.rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
			image.color = new Color(1f, 1f, 1f, 1f);
			clone.SetActive(value: true);
			LeanTween.rotateZ(clone, -359f, 1f);
			onMoveToStorage?.Invoke();
			LeanTween.move(clone, base.gameObject.transform.position, 1f).setOnComplete((Action)delegate
			{
				UnityEngine.Object.Destroy(clone);
				Impulse();
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.MoveToStorageSound.AudioClip);
			});
		}
	}
}
