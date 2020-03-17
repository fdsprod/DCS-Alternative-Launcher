using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Services
{
    public interface IAutoUpdateService
    {
        Task<AutoUpdateCheckResult> CheckAsync();
    }
}