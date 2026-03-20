using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace WhiteArrowEditor.Bootstraping
{
    public class BootSettingsWindow : EditorWindow
    {
        private static readonly Vector2 MinWindowSize = new(520f, 420f);

        private Editor _settingsEditor;



        [MenuItem("Tools/WhiteArrow/Bootstraping/Settings", priority = 0)]
        public static void Open()
        {
            var window = GetWindow<BootSettingsWindow>("Boot Settings");
            window.minSize = MinWindowSize;
            window.position = new Rect(window.position.position, MinWindowSize);
            window.Show();
        }



        private void OnEnable()
        {
            minSize = MinWindowSize;
            RebuildInspector();
        }

        private void OnDisable()
        {
            if (_settingsEditor != null)
                DestroyImmediate(_settingsEditor);
        }

        private void CreateGUI()
        {
            RebuildInspector();
        }



        private void RebuildInspector()
        {
            rootVisualElement.Clear();

            if (_settingsEditor != null)
            {
                DestroyImmediate(_settingsEditor);
                _settingsEditor = null;
            }

            var settings = EditorBootSettingsProvider.Settings;

            if (settings == null)
                return;

            _settingsEditor = Editor.CreateEditor(settings);

            var inspector = _settingsEditor.CreateInspectorGUI();
            if (inspector != null)
            {
                inspector.Bind(_settingsEditor.serializedObject);
                rootVisualElement.Add(inspector);
                return;
            }

            rootVisualElement.Add(new IMGUIContainer(_settingsEditor.OnInspectorGUI));
        }
    }
}
