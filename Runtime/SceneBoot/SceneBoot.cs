using System;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public abstract class SceneBoot : MonoBehaviour
    {
        public bool IsFinished { get; private set; }



        public event Action Finished;



        private void Start()
        {
            IsFinished = true;
            Run();
        }

        internal protected abstract void Run();



        protected void OnFinished()
        {
            IsFinished = true;
            Finished?.Invoke();
        }
    }
}