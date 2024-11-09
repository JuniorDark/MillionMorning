using System.Collections.Generic;
using Code.Core.Avatar;
using Code.World.Player;
using Core.GameEvent;
using Core.Input;
using UnityEngine;

namespace Core.Interaction;

public class InteractionManager : Singleton<InteractionManager>
{
	private const float AVATAR_COLLISION_RADIUS = 0.5f;

	private const double PRIO_RANGE = 0.5;

	private readonly IList<Interactable> _interactablesInRange = new List<Interactable>();

	private Interactable _currentInteractable;

	private Camera _camera;

	private int _prio;

	private bool _enabled = true;

	public bool HasCurrentInteractable => _currentInteractable != null;

	protected void Start()
	{
		_camera = Camera.main;
		if (_camera == null)
		{
			Debug.LogError(base.gameObject.name + ": Camera is null");
		}
		Core.GameEvent.GameEvent.OnUseEvent.RegisterAction(Interact);
	}

	private void Update()
	{
		if (!_enabled)
		{
			return;
		}
		if (_interactablesInRange.Count <= 0)
		{
			SwapToNewInteractable(null);
			return;
		}
		Interactable closestInteractable = GetClosestInteractable();
		if (!(closestInteractable == _currentInteractable))
		{
			SwapToNewInteractable(closestInteractable);
		}
	}

	private void OnDestroy()
	{
		Core.GameEvent.GameEvent.OnUseEvent.UnregisterAction(Interact);
	}

	private void SwapToNewInteractable(Interactable closest)
	{
		if (_currentInteractable != null)
		{
			_currentInteractable.ShowInteractionIcon(val: false);
		}
		_currentInteractable = closest;
		if (closest != null && !closest.Silent)
		{
			_currentInteractable.ShowInteractionIcon(val: true);
			Core.GameEvent.GameEvent.ShowUseWidgetEvent?.RaiseEvent(_currentInteractable.InteractionVerb);
		}
		else
		{
			Core.GameEvent.GameEvent.ShowUseWidgetEvent?.RaiseEvent("");
		}
	}

	private void Interact()
	{
		if (!(_currentInteractable == null))
		{
			_currentInteractable.InteractableObject.UseReaction();
		}
	}

	public bool InteractWithMouse()
	{
		if (!_camera)
		{
			Debug.LogError(base.gameObject.name + "(InteractMouse): Camera is null");
			return false;
		}
		if (!Physics.Raycast(_camera.ScreenPointToRay(InputSwitch.MousePosition), out var hitInfo, 10f, -33))
		{
			return false;
		}
		Interactable interactable = ((hitInfo.collider != null) ? hitInfo.collider.GetComponent<Interactable>() : null);
		if (interactable == null)
		{
			return false;
		}
		interactable.InteractableObject?.UseReaction();
		return true;
	}

	public void AddInteractableToNearby(Interactable interactable)
	{
		if (!_interactablesInRange.Contains(interactable))
		{
			_interactablesInRange.Add(interactable);
		}
	}

	public void RemoveInteractableFromNearby(Interactable interactable)
	{
		interactable.ShowInteractionIcon(val: false);
		_interactablesInRange.Remove(interactable);
	}

	public void SetMinimumPrio(int prio)
	{
		_prio = prio;
	}

	public void Enable()
	{
		_enabled = true;
	}

	public void Disable()
	{
		_enabled = false;
		if (_currentInteractable != null)
		{
			SwapToNewInteractable(null);
		}
	}

	private Interactable GetClosestInteractable()
	{
		if (_interactablesInRange.Count <= 0)
		{
			return null;
		}
		Interactable result = null;
		int num = int.MaxValue;
		float num2 = float.PositiveInfinity;
		MilMo_Player instance = MilMo_Player.Instance;
		if (instance == null || instance.IsTalking)
		{
			return null;
		}
		MilMo_Avatar avatar = instance.Avatar;
		if (avatar == null)
		{
			return null;
		}
		if (avatar.InHappyPickup)
		{
			return null;
		}
		for (int num3 = _interactablesInRange.Count - 1; num3 >= 0; num3--)
		{
			Interactable interactable = _interactablesInRange[num3];
			if (interactable == null)
			{
				_interactablesInRange.RemoveAt(num3);
			}
			else if (interactable.Prio < _prio)
			{
				_interactablesInRange.RemoveAt(num3);
			}
			else
			{
				Vector2 vector = new Vector2(interactable.ObjectPosition.x, interactable.ObjectPosition.z);
				Vector2 vector2 = new Vector2(avatar.Position.x, avatar.Position.z);
				float magnitude = (vector - vector2).magnitude;
				if (magnitude >= interactable.Radius + 0.5f)
				{
					if (_currentInteractable != null && _currentInteractable.Equals(interactable))
					{
						_currentInteractable = null;
					}
					RemoveInteractableFromNearby(interactable);
				}
				else
				{
					int prio = interactable.Prio;
					if ((double)magnitude < (double)num2 + 0.5 && prio <= num)
					{
						result = interactable;
						num2 = magnitude;
						num = prio;
					}
				}
			}
		}
		return result;
	}

	public static InteractionManager Get()
	{
		return Singleton<InteractionManager>.Instance;
	}
}
