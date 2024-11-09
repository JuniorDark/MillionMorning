using Code.Core.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements.Cooldown;

public class CooldownTimer : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private Image overlay;

	[SerializeField]
	private TMP_Text cooldownText;

	private IHaveCooldown _cooldownObject;

	private void Awake()
	{
		if (overlay == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing overlay");
		}
		if (cooldownText == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing cooldownText");
		}
	}

	private void Start()
	{
		Refresh();
	}

	public void Setup(IHaveCooldown itemWithCooldown)
	{
		_cooldownObject = itemWithCooldown;
		Refresh();
	}

	public void Clear()
	{
		_cooldownObject = null;
		Refresh();
	}

	public void Refresh()
	{
		if (_cooldownObject != null && !_cooldownObject.TestCooldownExpired())
		{
			Activate();
		}
		else
		{
			Deactivate();
		}
	}

	private void OnDisable()
	{
		overlay.fillAmount = 0f;
		cooldownText.text = "";
	}

	private void Activate()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	private void Deactivate()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void ApplyCooldown()
	{
		if (_cooldownObject == null || _cooldownObject.TestCooldownExpired())
		{
			Deactivate();
			return;
		}
		float timeRemaining = _cooldownObject.GetTimeRemaining();
		float cooldownProgress = _cooldownObject.GetCooldownProgress();
		if ((bool)cooldownText)
		{
			cooldownText.text = Mathf.RoundToInt(timeRemaining).ToString();
		}
		if ((bool)overlay)
		{
			overlay.fillAmount = 1f - cooldownProgress;
		}
	}

	private void Update()
	{
		ApplyCooldown();
	}
}
