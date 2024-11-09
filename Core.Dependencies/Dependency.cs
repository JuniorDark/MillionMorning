using System.Threading.Tasks;
using UnityEngine;

namespace Core.Dependencies;

public abstract class Dependency : MonoBehaviour, IDependency
{
	public abstract Task<bool> Check();
}
