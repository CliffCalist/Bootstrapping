namespace WhiteArrow.Bootstraping
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class BootModuleOrderAttribute : System.Attribute
    {
        public int Order { get; }

        public BootModuleOrderAttribute(int order = 0)
        {
            Order = order;
        }
    }
}
