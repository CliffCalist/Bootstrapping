using System.Threading.Tasks;

namespace WhiteArrow.Bootstraping
{
    public abstract class BootModule : AsyncBootModule
    {
        internal protected override sealed Task RunAsync()
        {
            Run();
            return Task.CompletedTask;
        }

        protected abstract void Run();
    }
}