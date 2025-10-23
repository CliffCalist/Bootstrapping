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
        [SerializeField] private VisualTreeAsset _modulesListView;
        [SerializeField] private VisualTreeAsset _moduleItemView;
        [SerializeField] private VisualTreeAsset _moduleItemButtonView;

        private BootSettings _settings;
        private VisualElement _modulesContainer;



        private void OnEnable()
        {
            _settings = (BootSettings)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            CreateInspectorWithoutModules(root);
            CreateModulesContainer(root);

            RefreshModulesContainer();
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

        private void CreateModulesContainer(VisualElement root)
        {
            var listRoot = _modulesListView.CloneTree();
            root.Add(listRoot);

            var addButton = listRoot.Q<Button>("addModule-button");
            addButton.clicked += () => ShowModulePicker();

            _modulesContainer = listRoot.Q("modules-container");
        }



        private void RefreshModulesContainer()
        {
            _modulesContainer.Clear();

            if (_settings.Modules.Count == 0)
            {
                var label = new Label("List is empty!")
                {
                    style =
                        {
                            marginBottom = 6,
                            marginTop = 6,
                            marginLeft = 6,
                            marginRight = 6
                        }
                };
                _modulesContainer.Add(label);
                return;
            }

            foreach (var module in _settings.Modules.ToList())
                CreateModuleFoldout(module);
        }

        private void CreateModuleFoldout(AsyncBootModule module)
        {
            var element = _moduleItemView.CloneTree();
            _modulesContainer.Add(element);

            var foldout = element.Q<Foldout>("module-foldout");
            foldout.text = module.GetType().Name;
            foldout.contentContainer.style.paddingTop = 6;

            var toggle = foldout.Q<Toggle>();
            toggle.style.marginLeft = 0;
            toggle.style.marginTop = 0;
            toggle.style.marginBottom = 0;
            toggle.style.backgroundColor = new Color(0.2352941F, 0.2352941F, 0.2352941F);

            var label = toggle?.Q<Label>();
            var foldoutHeader = label.parent;
            CreateModuleButtonGroup(module, foldoutHeader);

            var moduleEditor = CreateEditor(module);
            var editorElement = new IMGUIContainer(() => moduleEditor.OnInspectorGUI());
            foldout.Add(editorElement);
        }

        private void CreateModuleButtonGroup(AsyncBootModule module, VisualElement container)
        {
            var moveToUpButton = CreateModuleItemButton("↑", () =>
            {
                _settings.MoveModuleUp(module);
                EditorUtility.SetDirty(_settings);
                RefreshModulesContainer();
            });
            container.Add(moveToUpButton);

            var moveToDownButton = CreateModuleItemButton("↓", () =>
            {
                _settings.MoveModuleDown(module);
                EditorUtility.SetDirty(_settings);
                RefreshModulesContainer();
            });
            container.Add(moveToDownButton);

            var removeButton = CreateModuleItemButton("✖", () =>
            {
                _settings.RemoveModule(module);
                AssetDatabase.RemoveObjectFromAsset(module);

                DestroyImmediate(module, true);
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();

                RefreshModulesContainer();
            });
            removeButton.style.backgroundColor = Color.red;
            container.Add(removeButton);
        }

        private Button CreateModuleItemButton(string text, Action onClicked)
        {
            var container = _moduleItemButtonView.CloneTree();
            var button = container.Q<Button>();
            button.text = text;
            button.clicked += onClicked;
            return button;
        }



        private void ShowModulePicker()
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
                    new GUIContent(type.Name), false, () => CreateAndSetNewModule(type)
                );
            }

            menu.ShowAsContext();
        }

        private void CreateAndSetNewModule(Type moduleType)
        {
            var instance = CreateInstance(moduleType) as AsyncBootModule;
            if (instance == null)
                return;

            _settings.AddModule(instance);

            instance.name = moduleType.ToString();
            instance.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(instance, _settings);

            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();

            RefreshModulesContainer();
        }
    }
}