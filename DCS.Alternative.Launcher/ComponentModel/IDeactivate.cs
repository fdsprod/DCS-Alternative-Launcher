using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.ComponentModel
{
    public interface IDeactivate
    {
        bool IsActivated
        {
            get;
        }

        Task DeactivateAsync();
    }
}