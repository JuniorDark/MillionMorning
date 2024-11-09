using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements;

[RequireComponent(typeof(Toggle))]
public class SetToggleParent : MonoBehaviour
{
	private void OnEnable()
	{
		Toggle component = GetComponent<Toggle>();
		if (component == null)
		{
			Debug.LogError("{name}: Unable to get toggle from component");
			return;
		}
		ToggleGroup componentInParent = GetComponentInParent<ToggleGroup>();
		if (componentInParent == null)
		{
			Debug.LogError(base.name + ": Unable to get ToggleGroup from parent");
			return;
		}
		componentInParent.RegisterToggle(component);
		component.group = componentInParent;
	}
}
