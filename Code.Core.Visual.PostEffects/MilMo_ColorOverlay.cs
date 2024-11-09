using UnityEngine;

namespace Code.Core.Visual.PostEffects;

[RequireComponent(typeof(UnityEngine.Camera))]
public class MilMo_ColorOverlay : MonoBehaviour
{
	private Material _material;

	private Color _wantedColor = new Color(0f, 0f, 0f, 0f);

	private Color _currentColor = new Color(0f, 0f, 0f, 0f);

	public void GoToColor(Color color)
	{
		_wantedColor = color;
	}

	public void SnapColor(Color color)
	{
		_wantedColor = color;
		_currentColor = color;
	}

	private void Start()
	{
		Shader shader = Shader.Find("PostEffects/AdditiveColor");
		_material = new Material(shader);
	}

	private void Update()
	{
		_currentColor = Color.Lerp(_currentColor, _wantedColor, Time.deltaTime * 2f);
		_material.color = _currentColor;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, _material);
	}
}
