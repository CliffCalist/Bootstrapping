using System;

namespace WhiteArrow.Bootstraping
{
    public sealed class BootErrorContext
    {
        public string ModuleName { get; }
        public Exception Exception { get; }
        public bool IsCritical { get; }



        public BootErrorContext(string moduleName, Exception exception, bool isCritical)
        {
            ModuleName = moduleName ?? throw new ArgumentNullException(nameof(moduleName));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            IsCritical = isCritical;
        }
    }
}
