namespace Code.Core.Items.Home;

public abstract class MilMo_FurnitureStateAction
{
	public abstract void Activate<T>(T furniture);

	public abstract void Deactivate<T>(T furniture);
}
