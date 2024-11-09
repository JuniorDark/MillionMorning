using Code.Core.ResourceSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Elements;

public class ProfanityFilter : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField input;

	[SerializeField]
	private UnityEvent validityFail;

	[SerializeField]
	private UnityEvent validityPass;

	private void Awake()
	{
		if (input == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing input");
		}
	}

	private void Start()
	{
		MilMo_BadWordFilter.AsyncInit();
		input.onValueChanged.AddListener(CheckValidity);
	}

	private void CheckValidity(string word)
	{
		if (MilMo_BadWordFilter.IsReady && !string.IsNullOrEmpty(word))
		{
			if (MilMo_BadWordFilter.GetStringIntegrity(word) != MilMo_BadWordFilter.StringIntegrity.OK)
			{
				validityFail?.Invoke();
				Debug.LogWarning("Profanity check failed");
			}
			else
			{
				validityPass?.Invoke();
			}
		}
	}
}
