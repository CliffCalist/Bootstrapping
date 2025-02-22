using System;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public abstract class SceneBoot : MonoBehaviour
    {
        public abstract void Run(Action onEnded);
    }
}