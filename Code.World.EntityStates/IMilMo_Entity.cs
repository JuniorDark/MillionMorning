namespace Code.World.EntityStates;

public interface IMilMo_Entity
{
	void AddStateEffect(string particleEffect);

	void RemoveStateEffect(string particleEffect);

	MilMo_EntityStateManager GetEntityStateManager();
}
