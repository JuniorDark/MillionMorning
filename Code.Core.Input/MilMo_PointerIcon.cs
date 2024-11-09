using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Input;

public sealed class MilMo_PointerIcon : MonoBehaviour
{
	private const int ICON_SIZE = 32;

	private MilMo_Texture _openHandTexture;

	private MilMo_Texture _closedHandTexture;

	private bool _isDeactivating;

	private MilMo_Texture CurrentTexture { get; set; }

	private bool Enabled { get; set; }

	private float WantedAlpha { get; set; }

	private float Alpha { get; set; }

	public void Awake()
	{
		_openHandTexture = new MilMo_Texture("Content/GUI/Batch01/Textures/Homes/IconGrab");
		_openHandTexture.AsyncLoad();
		_closedHandTexture = new MilMo_Texture("Content/GUI/Batch01/Textures/WorldMap/IconGrabMap");
		_closedHandTexture.AsyncLoad();
	}

	public void SetEnabled(bool e)
	{
		if (e)
		{
			Activate();
		}
		else
		{
			Deactivate();
		}
	}

	private void Activate()
	{
		if (!Enabled)
		{
			SetAlpha(0);
			Enabled = true;
			AlphaTo(1);
		}
	}

	private void Deactivate()
	{
		if (!_isDeactivating && Enabled)
		{
			AlphaTo(0);
			MilMo_EventSystem.At(0.2f, delegate
			{
				Enabled = false;
				_isDeactivating = false;
			});
			_isDeactivating = true;
		}
	}

	private void AlphaTo(int i)
	{
		WantedAlpha = i;
	}

	private void SetAlpha(int i)
	{
		Alpha = i;
		WantedAlpha = i;
	}

	public void OnGUI()
	{
		if (!Enabled)
		{
			return;
		}
		CurrentTexture = (MilMo_Pointer.LeftButton ? _closedHandTexture : _openHandTexture);
		if (CurrentTexture != null && !(CurrentTexture.Texture == null))
		{
			Rect position = new Rect(MilMo_Pointer.Position.x - 16f, MilMo_Pointer.Position.y - 16f, 32f, 32f);
			UnityEngine.GUI.color *= Alpha;
			if (Alpha > WantedAlpha + 0.01f)
			{
				Alpha -= 0.01f;
			}
			if (Alpha < WantedAlpha - 0.01f)
			{
				Alpha += 0.01f;
			}
			UnityEngine.GUI.depth = 0;
			UnityEngine.GUI.DrawTexture(position, CurrentTexture.Texture, ScaleMode.StretchToFill, alphaBlend: true, 0f);
		}
	}
}
