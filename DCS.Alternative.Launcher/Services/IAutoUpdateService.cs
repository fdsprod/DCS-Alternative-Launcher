using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.Services
{
    public interface IAutoUpdateService
    {
        Task<bool> CheckAsync();
    }
}
