using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.Threading
{
    public abstract class HandlerTaskSourceBase<T>
    {
        public abstract Task<T> Task
        {
            get;
        }
    }
}