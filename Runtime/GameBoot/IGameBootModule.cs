using System.Threading.Tasks;

namespace WhiteArrow.Bootstraping
{
    public interface IGameBootModule : IAsyncGameBootModule
    {
        Task IAsyncGameBootModule.RunAsync()
        {
            Run();
            return Task.CompletedTask;
        }

        void Run();
    }
}