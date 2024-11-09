using System.Collections.Generic;
using System.Linq;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.Player;
using Core;
using Core.GameEvent;
using Core.Utilities;
using UI;
using UI.Bubbles;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Player;

public class ThinkManager : Singleton<ThinkManager>
{
	[SerializeField]
	private AssetReference thinkBubble;

	private Transform _containerTransform;

	private readonly Queue<string> _queue = new Queue<string>();

	private string _currentThought;

	private ThinkBubble _bubble;

	private MilMo_Player PlayerInstance => MilMo_Player.Instance;

	private void Awake()
	{
		GameEvent.ThinkEvent.RegisterAction(AddThought);
	}

	private void OnDestroy()
	{
		GameEvent.ThinkEvent.UnregisterAction(AddThought);
	}

	protected void Start()
	{
		GameObject container = WorldSpaceManager.GetContainer();
		if (container == null)
		{
			Debug.LogError(base.gameObject.name + ": missing container");
		}
		else
		{
			_containerTransform = container.transform;
		}
	}

	public void AddThought(string thoughtText)
	{
		if (!(_currentThought == thoughtText) && !_queue.Any((string queuedText) => queuedText == thoughtText))
		{
			_queue.Enqueue(thoughtText);
		}
	}

	private void Update()
	{
		if (string.IsNullOrEmpty(_currentThought) && _queue.Count >= 1)
		{
			PlayerThinks(_currentThought = _queue.Dequeue());
		}
	}

	public void ClearHead()
	{
		if (_bubble != null)
		{
			_bubble.Hide();
		}
	}

	public void PlayerThinks(string thought, UnityAction done = null)
	{
		_bubble = Instantiator.Instantiate<ThinkBubble>(thinkBubble, _containerTransform);
		if (!(_bubble == null))
		{
			_bubble.SetCallback(delegate
			{
				_currentThought = "";
				_bubble = null;
				done?.Invoke();
				PlayerInstance.UpdateIsTalking(isTalking: false);
			});
			_bubble.SetText(thought);
			Transform transform = ((PlayerInstance?.Avatar != null && PlayerInstance?.Avatar.Head != null) ? PlayerInstance.Avatar.Head : null);
			if (transform != null)
			{
				_bubble.SetTarget(transform);
			}
			_bubble.Show();
		}
	}

	public void PlayerThinksFromServer(NpcMessagePart part)
	{
		string text = part.GetLines()?.First();
		if (!string.IsNullOrEmpty(text))
		{
			string @string = MilMo_Localization.GetLocString(text).String;
			PlayerThinks(@string);
		}
	}
}
