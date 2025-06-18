using System.Threading.Tasks;

namespace WhiteArrow.Bootstraping
{
    public interface IBootModule : IAsyncBootModule
    {
        Task IAsyncBootModule.RunAsync()
        {
            Run();
            return Task.CompletedTask;
        }

        void Run();
    }
}