using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public abstract class AsyncBootModule : ScriptableObject
    {
        internal protected abstract Task RunAsync();
    }
}