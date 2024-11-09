using System.Threading.Tasks;

namespace Core.Dependencies;

public interface IDependency
{
	Task<bool> Check();
}
