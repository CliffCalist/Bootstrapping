using System;
using UnityEngine;
using Zenject;

namespace WhiteArrow.Bootstraping
{
    public abstract class SceneBoot : MonoBehaviour
    {
        public static DiContainer RootDiContainer { get; private set; }


        internal void Run(DiContainer sceneDiContainer, Action onEnded)
        {
            if (onEnded is null)
                throw new ArgumentNullException(nameof(onEnded));

            RootDiContainer = sceneDiContainer ?? throw new ArgumentNullException(nameof(sceneDiContainer));
            OnRun(sceneDiContainer, onEnded);
        }

        protected abstract void OnRun(DiContainer sceneDiContainer, Action onEnded);
    }
}