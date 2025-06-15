namespace WhiteArrow.Bootstraping
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GameBootOrderAttribute : System.Attribute
    {
        public int Order { get; }

        public GameBootOrderAttribute(int order = 0)
        {
            Order = order;
        }
    }
}
