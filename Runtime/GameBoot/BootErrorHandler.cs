using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WhiteArrow.Bootstraping
{
    public abstract class BootErrorHandler : MonoBehaviour
    {
        public abstract Task OnErrorAsync(BootErrorContext context);

        public virtual Task SelfDestroyAsync()
        {
            Object.Destroy(gameObject);
            return Task.CompletedTask;
        }
    }
}
