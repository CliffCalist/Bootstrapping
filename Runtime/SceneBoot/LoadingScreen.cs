using System;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public abstract class LoadingScreen : MonoBehaviour
    {
        public abstract bool IsShowed { get; }

        public abstract void Show(bool skipAnimations, Action callback);
        public abstract void Hide();
    }
}