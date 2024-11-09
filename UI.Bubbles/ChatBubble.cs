using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Bubbles;

public class ChatBubble : TextBubble
{
	[Serializable]
	public struct BubbleSize
	{
		public int letterCount;

		public Vector2 size;
	}

	private const float LIFE_TIME_PER_LETTER = 0.1f;

	[SerializeField]
	private RectTransform bubble;

	[SerializeField]
	private List<BubbleSize> bubbleSizes = new List<BubbleSize>();

	private int _currentTextLength;

	protected override void Awake()
	{
		base.Awake();
		if (bubble == null)
		{
			Debug.LogWarning(base.gameObject.name + ": bubble is missing");
		}
	}

	protected override void RefreshSize()
	{
		int textLength = GetTextLength();
		if (_currentTextLength == textLength)
		{
			return;
		}
		_currentTextLength = textLength;
		lifeTime = Mathf.Clamp(0.1f * (float)textLength, 1f, 10f);
		if (bubbleSizes.Count >= 1)
		{
			Vector2 size = bubbleSizes.First((BubbleSize bubbleSize) => textLength < bubbleSize.letterCount).size;
			if (!(size == Vector2.zero))
			{
				bubble.sizeDelta = size;
			}
		}
	}
}
