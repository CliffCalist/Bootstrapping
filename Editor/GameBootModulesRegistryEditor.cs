using UnityEditor;
using UnityEngine;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    [CustomEditor(typeof(GameBootModulesRegistry))]
    public class GameBootModulesRegistryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}