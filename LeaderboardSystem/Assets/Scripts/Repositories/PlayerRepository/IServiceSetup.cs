using System.Threading.Tasks;

namespace Repositories
{
    public interface IServiceSetup
    {
        Task WaitCheckForSetup();
    }
}
