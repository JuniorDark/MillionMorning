using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltip.Stars;

public class Star : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private Image fill;

	private const float FILL_PER_PART_PIECE = 0.5f;

	private const int PIECES_PER_PART = 2;

	public int EmptyPieces => 2 - CalculateFilledPartPieces();

	public void OnEnable()
	{
		SetFillAmount(0f);
	}

	public void SetFillAmount(float fillAmount)
	{
		fill.fillAmount = fillAmount;
	}

	private int CalculateFilledPartPieces()
	{
		return (int)(fill.fillAmount * 2f);
	}

	public void Replenish(int numberOfPieces)
	{
		if (numberOfPieces < 0)
		{
			throw new ArgumentOutOfRangeException("numberOfPieces");
		}
		fill.fillAmount += (float)numberOfPieces * 0.5f;
	}
}
