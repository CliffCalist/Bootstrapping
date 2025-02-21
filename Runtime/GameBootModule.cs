using Zenject;

namespace WhiteArrow.Bootstraping
{
    public abstract class GameBootModule
    {
        public abstract void Run(DiContainer gameDiContainer);
    }
}
