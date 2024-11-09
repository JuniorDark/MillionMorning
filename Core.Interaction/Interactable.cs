using Core.Utilities;
using UI.FX;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Core.Interaction;

public class Interactable : MonoBehaviour
{
	public enum InteractionType
	{
		Silver,
		Gold,
		Pickup,
		PickDown,
		HighlightOnly
	}

	private const double INTERACTION_COOLDOWN = 0.2;

	private const string SILVER_MARKER_ADDRESS = "NPCMarkerSilver";

	private const string GOLD_MARKER_ADDRESS = "NPCMarkerSilver";

	private const string DOWN_ARROW = "DownArrow";

	private const string UP_ARROW = "UpArrow";

	[Header("Scene")]
	[SerializeField]
	private Collider interactionCollider;

	[SerializeField]
	private Image interactionMarkerIcon;

	private Sprite _silverMarkerSprite;

	private Sprite _goldMarkerSprite;

	private Sprite _downArrowSprite;

	private Sprite _upArrowSprite;

	private InteractionManager _interactionManager;

	private bool _autoTrigger;

	private bool _isInside;

	private float _leaveTriggerTime;

	private UIAlphaFX _fader;

	public IHasInteraction InteractableObject { get; private set; }

	public bool Silent { get; private set; }

	public Vector3 ObjectPosition => InteractableObject?.GetPosition() ?? Vector3.zero;

	public float Radius => GetRadius();

	public int Prio => InteractableObject?.GetPrio() ?? int.MaxValue;

	public string InteractionVerb => InteractableObject?.GetInteractionVerb() ?? "";

	private void Awake()
	{
		_interactionManager = InteractionManager.Get();
		if (_interactionManager == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _interactionManager is null");
			return;
		}
		InitializeAddressables();
		if (!(interactionCollider == null))
		{
			Collider collider = interactionCollider;
			if (collider is CapsuleCollider || collider is SphereCollider)
			{
				if (interactionMarkerIcon == null)
				{
					Debug.LogWarning(base.gameObject.name + ": interactionMarkerIcon is null");
					return;
				}
				interactionMarkerIcon.gameObject.transform.localScale = Vector3.zero;
				_fader = GetComponent<UIAlphaFX>();
				if (_fader == null)
				{
					Debug.LogWarning(base.gameObject.name + ": _fader is missing");
				}
				else
				{
					_fader.FadeOutFast();
				}
				return;
			}
		}
		Debug.LogWarning(base.gameObject.name + ": interactionCollider is null or not Capsule/Sphere");
	}

	private void OnDestroy()
	{
		if (_interactionManager != null)
		{
			_interactionManager.RemoveInteractableFromNearby(this);
		}
	}

	public void Initialize(IHasInteraction interactableObject, float interactionRadius, bool silent = false, bool autoTrigger = false)
	{
		InteractableObject = interactableObject;
		Silent = silent;
		_autoTrigger = autoTrigger;
		ShowInteractionIcon(val: false);
		SetRadius(interactionRadius);
		SetInteractionMarkerType(InteractableObject.GetInteractionType());
		SetMarkerOffset(InteractableObject.GetMarkerOffset());
	}

	private void InitializeAddressables()
	{
		_silverMarkerSprite = Addressables.LoadAssetAsync<Sprite>("NPCMarkerSilver").WaitForCompletion();
		if (!_silverMarkerSprite)
		{
			Debug.LogError("Unable to load SilverMarkerSprite");
		}
		_goldMarkerSprite = Addressables.LoadAssetAsync<Sprite>("NPCMarkerSilver").WaitForCompletion();
		if (!_goldMarkerSprite)
		{
			Debug.LogError("Unable to load GoldMarkerSprite");
		}
		_downArrowSprite = Addressables.LoadAssetAsync<Sprite>("DownArrow").WaitForCompletion();
		if (!_downArrowSprite)
		{
			Debug.LogError("Unable to load DownArrowSprite");
		}
		_upArrowSprite = Addressables.LoadAssetAsync<Sprite>("UpArrow").WaitForCompletion();
		if (!_downArrowSprite)
		{
			Debug.LogError("Unable to load DownArrowSprite");
		}
	}

	public void ShowInteractionIcon(bool val)
	{
		if (interactionMarkerIcon == null)
		{
			return;
		}
		if (Silent && interactionMarkerIcon.enabled)
		{
			interactionMarkerIcon.enabled = false;
			return;
		}
		if (InteractableObject is IHasHighlight hasHighlight)
		{
			hasHighlight.ShowHighlight(val);
		}
		if (!(_fader == null))
		{
			if (val)
			{
				_fader.FadeIn();
			}
			else
			{
				_fader.FadeOut();
			}
		}
	}

	public void SetInteractionMarkerType(InteractionType? type)
	{
		if (!(interactionMarkerIcon == null))
		{
			if (type == InteractionType.HighlightOnly)
			{
				interactionMarkerIcon.enabled = false;
				return;
			}
			Image image = interactionMarkerIcon;
			image.sprite = type switch
			{
				InteractionType.Silver => _silverMarkerSprite, 
				InteractionType.Gold => _goldMarkerSprite, 
				InteractionType.Pickup => _upArrowSprite, 
				InteractionType.PickDown => _downArrowSprite, 
				_ => interactionMarkerIcon.sprite, 
			};
		}
	}

	private void SetMarkerOffset(Vector3 localPosition)
	{
		if (!(interactionMarkerIcon == null))
		{
			interactionMarkerIcon.transform.localPosition = localPosition;
		}
	}

	private float GetRadius()
	{
		if (interactionCollider == null)
		{
			return 0f;
		}
		Collider collider = interactionCollider;
		if (!(collider is SphereCollider { radius: var radius }))
		{
			if (!(collider is CapsuleCollider { radius: var radius2 }))
			{
				return 0f;
			}
			return radius2;
		}
		return radius;
	}

	private void SetRadius(float radius)
	{
		if (!(interactionCollider == null))
		{
			if (interactionCollider is SphereCollider sphereCollider)
			{
				sphereCollider.radius = radius;
			}
			if (interactionCollider is CapsuleCollider capsuleCollider)
			{
				capsuleCollider.radius = radius;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (PlayerUtils.IsLocalPlayer(other.gameObject) && !((double)Time.time - 0.2 < (double)_leaveTriggerTime) && !_isInside)
		{
			_isInside = true;
			if (_autoTrigger)
			{
				InteractableObject.UseReaction();
			}
			else if (!(_interactionManager == null))
			{
				_interactionManager.AddInteractableToNearby(this);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (PlayerUtils.IsLocalPlayer(other.gameObject))
		{
			_leaveTriggerTime = Time.time;
			_isInside = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, Radius);
	}
}
