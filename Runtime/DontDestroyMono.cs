using System.Collections;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    internal class DontDestroyMono : MonoBehaviour
    {
        private static DontDestroyMono s_instance;


        private static DontDestroyMono Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var instanceObject = new GameObject("[BOOTSTRAPING]");
                    s_instance = instanceObject.AddComponent<DontDestroyMono>();

                    instanceObject.hideFlags = HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(instanceObject);
                }

                return s_instance;
            }
        }



        internal static void LaunchCoroutine(IEnumerator routine)
        {
            Instance.StartCoroutine(routine);
        }

        internal static void AddChild(Transform child)
        {
            child.SetParent(Instance.transform);
        }
    }
}