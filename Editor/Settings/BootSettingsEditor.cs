using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstrapping
{
    [CustomEditor(typeof(BootSettings))]
    public class BootSettingsEditor : Editor
    {
        private BootSettings _settings;



        private void OnEnable()
        {
            _settings = (BootSettings)target;
        }



        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            CreateInspectorWithoutModules(root);

            var modulesList = new FlexList();
            modulesList.Label.text = "Modules";

            modulesList.SetItemsSource(
                _settings.Modules,
                new ModuleCreator(_settings),
                item => RemoveModule(item as AsyncBootModule),
                item => RenderModule(item as AsyncBootModule)
            );

            modulesList.EnableItemReordering((item, newIndex) =>
            {
                var module = item as AsyncBootModule;
                _settings.MoveModule(module, newIndex);
            });

            modulesList.GetItemName = item => GetModuleName(item as AsyncBootModule);

            modulesList.Refresh();
            root.Add(modulesList);

            return root;
        }

        private void CreateInspectorWithoutModules(VisualElement root)
        {
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.propertyPath == "_modules" || iterator.propertyPath == "m_Script")
                    continue;

                root.Add(new PropertyField(iterator.Copy()));
            }
        }



        private void RemoveModule(AsyncBootModule module)
        {
            Undo.RecordObject(_settings, "Remove Module");

            _settings.RemoveModule(module);
            AssetDatabase.RemoveObjectFromAsset(module);
            EditorUtility.SetDirty(_settings);

            DestroyImmediate(module, true);
        }

        private string GetModuleName(AsyncBootModule module)
        {
            var type = module.GetType();
            return type.Name;
        }

        private VisualElement RenderModule(AsyncBootModule module)
        {
            var moduleEditor = CreateEditor(module);
            var editorElement = new IMGUIContainer(() => moduleEditor.OnInspectorGUI());
            return editorElement;
        }



        private class ModuleCreator : IFlexItemCreator
        {
            private readonly BootSettings _settings;



            public ModuleCreator(BootSettings settings)
            {
                _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            }

            public void RequestCreate(Action<bool> onComplete)
            {
                var types = TypeCache.GetTypesDerivedFrom<AsyncBootModule>()
                    .Where(t => !t.IsAbstract && !_settings.Modules.Any(m => m.GetType() == t))
                    .Where(t => !t.IsAbstract)
                    .ToList();

                if (types.Count == 0)
                {
                    EditorUtility.DisplayDialog("No Modules", "All available modules are already added.", "OK");
                    return;
                }

                var menu = new GenericMenu();
                foreach (var type in types)
                {
                    menu.AddItem(
                        new GUIContent(type.Name), false, () =>
                        {
                            CreateAndSetNewModule(type);
                            onComplete(true);
                        }
                    );
                }

                menu.ShowAsContext();
            }

            private void CreateAndSetNewModule(Type moduleType)
            {
                var instance = CreateInstance(moduleType) as AsyncBootModule;
                if (instance == null)
                    return;

                Undo.RecordObject(_settings, $"Add {moduleType} Module");

                _settings.AddModule(instance);

                instance.name = moduleType.ToString();
                instance.hideFlags = HideFlags.HideInHierarchy;

                AssetDatabase.AddObjectToAsset(instance, _settings);
                EditorUtility.SetDirty(_settings);
            }
        }
    }
}