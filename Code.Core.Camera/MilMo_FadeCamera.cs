using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_FadeCamera : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.6f, 0.7f, -1.8f, -1.2f), new Keyframe(1f, 0f));

	[SerializeField]
	[Range(0f, 1f)]
	private float alpha;

	private static int _fadeAmountID;

	private float _time;

	private Material _material;

	private void Awake()
	{
		Shader shader = Shader.Find("ImageEffects/ScreenFader");
		_fadeAmountID = Shader.PropertyToID("_FadeAmount");
		_material = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	private void OnEnable()
	{
		_time = 0f;
	}

	private void Start()
	{
		Reset();
	}

	private void Update()
	{
		_time += Time.deltaTime;
		alpha = fadeCurve.Evaluate(_time);
		if (alpha <= 0f)
		{
			base.enabled = false;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		_material.SetFloat(_fadeAmountID, 1f - alpha);
		Graphics.Blit(source, destination, _material);
	}

	public void FadeIn(float duration)
	{
		fadeCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(duration, 0f));
		Reset();
	}

	public void FadeOut(float duration)
	{
		fadeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(duration, 1f));
		Reset();
	}

	private void Reset()
	{
		base.enabled = true;
		_time = 0f;
	}
}
