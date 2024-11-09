using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Analytics;
using Core.Settings;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Core;

public class InitializerManager : MonoBehaviour
{
	private const string INVALID = "invalid";

	private const string RELEASE = "release";

	private const string BETA = "beta";

	private const string ALPHA = "alpha";

	private const string EDITOR = "editor";

	private const string PRODUCTION = "production";

	private string _consentIdentifier;

	private bool _isOptInConsentRequired;

	private async void Awake()
	{
		string text = "production";
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Core.Settings.Settings.Init();
		try
		{
			Debug.Log("Connecting to Analytics");
			InitializationOptions initializationOptions = new InitializationOptions();
			Debug.Log("Setting environment: " + text);
			initializationOptions.SetEnvironmentName(text);
			await UnityServices.InitializeAsync(initializationOptions);
			await AnalyticsCheckConsent();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private async Task AnalyticsCheckConsent()
	{
		IAnalyticsService service = AnalyticsService.Instance;
		List<string> list = await service.CheckForRequiredConsents();
		if (list.Count > 0)
		{
			_consentIdentifier = list[0];
			_isOptInConsentRequired = _consentIdentifier == "pipl";
		}
		if (_isOptInConsentRequired)
		{
			Debug.Log("Opted out of Analytics due to PIPL");
			service.ProvideOptInConsent(_consentIdentifier, consent: false);
			MilMoAnalyticsHandler.IsEnabled = false;
		}
	}
}
