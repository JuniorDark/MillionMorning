using UnityEngine;
using UnityEngine.Localization;

namespace Localization;

[CreateAssetMenu(fileName = "newLocaleBridgeIdentifier", menuName = "Localization/New Locale Identifier Bridge")]
public class LocaleIdentifierBridgeSO : ScriptableObject
{
	public LocaleIdentifier localeIdentifier;

	public string folderName;
}
