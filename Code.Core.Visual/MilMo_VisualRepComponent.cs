using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_VisualRepComponent : MonoBehaviour
{
	[SerializeField]
	private MilMo_VisualRep visualRep;

	[SerializeField]
	private MilMo_VisualRepData data;

	public MilMo_VisualRep GetVisualRep()
	{
		return visualRep;
	}

	public MilMo_VisualRepData GetData()
	{
		return data;
	}

	public void SetVisualRep(MilMo_VisualRep newVisualRep)
	{
		visualRep = newVisualRep;
	}

	public void SetData(MilMo_VisualRepData newData)
	{
		data = newData;
	}
}
