using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public abstract class SceneBoot : MonoBehaviour
    {
        internal protected virtual Task PrepareSceneAsync()
        {
            return Task.CompletedTask;
        }

        internal protected abstract Task InitializeSceneAsync();
    }
}