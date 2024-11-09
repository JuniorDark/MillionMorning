using Code.Core.Items.Home;
using Code.Core.Network.types;

namespace Code.World.Home.FurnitureActions;

public static class MilMo_ActionCreator
{
	public static MilMo_FurnitureStateAction CreateAction(FurnitureStateAction action)
	{
		if (!(action.GetTemplateType() == "ObjectEffect"))
		{
			return null;
		}
		return new MilMo_ActionObjectEffect((ActionObjectEffect)action);
	}
}
