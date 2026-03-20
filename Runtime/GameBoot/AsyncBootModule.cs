using System;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public abstract class AsyncBootModule : ScriptableObject
    {
        internal protected abstract BootFailureAction FailureAction { get; }

        internal protected abstract Task RunAsync();

        internal protected virtual Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }
    }
}
