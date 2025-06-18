using System.Threading.Tasks;

namespace WhiteArrow.Bootstraping
{
    public interface IAsyncBootModule
    {
        Task RunAsync();
    }
}