using System.Threading.Tasks;

namespace Repositories
{
    public interface ILeaderboardRepository
    {
        Task SaveChangesAsync();
        Task LoadScoresAsync();
        Task DeleteFileAsync();
    }
}
