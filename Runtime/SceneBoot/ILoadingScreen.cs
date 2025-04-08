using System;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public interface ILoadingScreen
    {
        public GameObject SelfObject { get; }
        public bool IsShowed { get; }

        public void Show(Action callback);
        public void Hide();
    }
}
