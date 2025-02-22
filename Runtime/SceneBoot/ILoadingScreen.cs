using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public interface ILoadingScreen
    {
        public GameObject SelfObject { get; }

        public void Show();
        public void Hide();
    }
}
