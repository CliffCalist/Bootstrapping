using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public abstract class SceneBoot : MonoBehaviour
    {
        internal protected abstract Task RunAsync();
    }
}