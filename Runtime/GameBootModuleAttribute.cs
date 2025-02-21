namespace WhiteArrow.Bootstraping
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GameBootModuleAttribute : System.Attribute
    {
        public int Order { get; }

        public GameBootModuleAttribute(int order = 0)
        {
            Order = order;
        }
    }
}
