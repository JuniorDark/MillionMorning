using UnityEngine;

namespace Core.Factory;

public abstract class FactorySO<T> : ScriptableObject, IFactory<T>
{
	public abstract T Create();
}
