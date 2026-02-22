using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IdaelDev.DependencyInjection.Editor
{
    public class DIContainerInspectorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private string _searchFilter = "";
        private Lifetime? _lifetimeFilter;
        private string _scopeFilter;
        private bool _showOnlySceneObjects;
        private readonly Dictionary<object, bool> _foldoutStates = new Dictionary<object, bool>();

        private GUIStyle _instanceStyle;
        private GUIStyle _dependencyStyle;
        private bool _stylesInitialized;

        [MenuItem("Window/DI Container Inspector")]
        public static void ShowWindow()
        {
            var window = GetWindow<DIContainerInspectorWindow>("DI Inspector");
            window.minSize = new Vector2(400, 300);
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                _foldoutStates.Clear();
            }
        }

        private void InitializeStyles()
        {
            if (_stylesInitialized) return;

            new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                margin = new RectOffset(5, 5, 10, 5)
            };

            _instanceStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 5, 5),
                margin = new RectOffset(5, 5, 2, 2)
            };

            _dependencyStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                padding = new RectOffset(15, 5, 2, 2)
            };

            _stylesInitialized = true;
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("DI Container Inspector is only available in Play Mode", MessageType.Info);
                return;
            }

            InitializeStyles();

            // Utiliser BeginVertical pour empiler verticalement
            EditorGUILayout.BeginVertical();

            DrawToolbar();

            EditorGUILayout.Space(10);

            DrawStatistics();

            EditorGUILayout.Space(10);

            DrawInstancesList();

            EditorGUILayout.EndVertical();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Filters", EditorStyles.boldLabel);

            EditorGUILayout.Space(5);

            // Search bar
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(60));
            _searchFilter = EditorGUILayout.TextField(_searchFilter);
            if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(50)))
            {
                _searchFilter = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Lifetime filter
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Lifetime:", GUILayout.Width(60));

            var lifetimeOptions = new[] { "All", "Singleton", "Transient", "Scoped" };
            var selectedLifetime = _lifetimeFilter == null ? 0 : (int)_lifetimeFilter.Value + 1;
            selectedLifetime = GUILayout.Toolbar(selectedLifetime, lifetimeOptions);

            _lifetimeFilter = selectedLifetime == 0 ? null : (Lifetime)(selectedLifetime - 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Scope filter
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scope:", GUILayout.Width(60));

            var scopeNames = DIContainerTracker.Instance.GetActiveScopeNames().ToList();
            scopeNames.Insert(0, "All");

            var scopeIndex = _scopeFilter == null ? 0 : scopeNames.IndexOf(_scopeFilter);
            if (scopeIndex < 0) scopeIndex = 0;

            scopeIndex = EditorGUILayout.Popup(scopeIndex, scopeNames.ToArray());
            _scopeFilter = scopeIndex == 0 ? null : scopeNames[scopeIndex];
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Scene objects only toggle
            _showOnlySceneObjects = EditorGUILayout.Toggle("Scene Objects Only", _showOnlySceneObjects);

            EditorGUILayout.Space(5);

            // Refresh button
            if (GUILayout.Button("Refresh", GUILayout.Height(25)))
            {
                Repaint();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawStatistics()
        {
            var instances = DIContainerTracker.Instance.GetAllInstances().ToList();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Statistics", EditorStyles.boldLabel);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Total: {instances.Count}", GUILayout.Width(100));
            EditorGUILayout.LabelField($"Singleton: {instances.Count(i => i.Lifetime == Lifetime.Singleton)}", GUILayout.Width(100));
            EditorGUILayout.LabelField($"Transient: {instances.Count(i => i.Lifetime == Lifetime.Transient)}", GUILayout.Width(100));
            EditorGUILayout.LabelField($"Scoped: {instances.Count(i => i.Lifetime == Lifetime.Scoped)}", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Scene Objects: {instances.Count(i => i.IsFromScene)}", GUILayout.Width(150));
            EditorGUILayout.LabelField($"Active Scopes: {DIContainerTracker.Instance.GetActiveScopeNames().Count()}", GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawInstancesList()
        {
            var instances = GetFilteredInstances();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label($"Instances ({instances.Count})", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // ScrollView pour la liste
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            if (instances.Count == 0)
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.HelpBox("No instances found matching the current filters", MessageType.Info);
            }
            else
            {
                foreach (var instanceInfo in instances)
                {
                    DrawInstance(instanceInfo);
                }

                EditorGUILayout.Space(10);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        private List<InstanceInfo> GetFilteredInstances()
        {
            var instances = DIContainerTracker.Instance.GetAllInstances().ToList();

            // Apply lifetime filter
            if (_lifetimeFilter.HasValue)
            {
                instances = instances.Where(i => i.Lifetime == _lifetimeFilter.Value).ToList();
            }

            // Apply scope filter
            if (!string.IsNullOrEmpty(_scopeFilter))
            {
                instances = instances.Where(i => i.ScopeName == _scopeFilter).ToList();
            }

            // Apply scene objects filter
            if (_showOnlySceneObjects)
            {
                instances = instances.Where(i => i.IsFromScene).ToList();
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(_searchFilter))
            {
                instances = instances.Where(i =>
                    i.ServiceType.Name.ToLower().Contains(_searchFilter.ToLower()) ||
                    i.ImplementationType.Name.ToLower().Contains(_searchFilter.ToLower())
                ).ToList();
            }

            return instances.OrderBy(i => i.ServiceType.Name).ToList();
        }

        private void DrawInstance(InstanceInfo instanceInfo)
        {
            EditorGUILayout.BeginVertical(_instanceStyle);

            // Header with foldout
            EditorGUILayout.BeginHorizontal();

            _foldoutStates.TryAdd(instanceInfo.Instance, false);

            _foldoutStates[instanceInfo.Instance] = EditorGUILayout.Foldout(
                _foldoutStates[instanceInfo.Instance],
                instanceInfo.ServiceType.Name,
                true,
                EditorStyles.foldoutHeader
            );

            // Lifetime badge
            var lifetimeColor = GetLifetimeColor(instanceInfo.Lifetime);
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = lifetimeColor;
            GUILayout.Label(instanceInfo.Lifetime.ToString(), EditorStyles.miniButton, GUILayout.Width(80));
            GUI.backgroundColor = prevColor;

            // Scene badge
            if (instanceInfo.IsFromScene)
            {
                GUI.backgroundColor = new Color(0.5f, 0.8f, 1f);
                GUILayout.Label("Scene", EditorStyles.miniButton, GUILayout.Width(50));
                GUI.backgroundColor = prevColor;
            }

            EditorGUILayout.EndHorizontal();

            // Details (if expanded)
            if (_foldoutStates[instanceInfo.Instance])
            {
                EditorGUI.indentLevel++;

                // Implementation type
                if (instanceInfo.ServiceType != instanceInfo.ImplementationType)
                {
                    EditorGUILayout.LabelField("Implementation:", instanceInfo.ImplementationType.Name);
                }

                // Scope
                EditorGUILayout.LabelField("Scope:", instanceInfo.ScopeName);

                // Created at
                EditorGUILayout.LabelField("Created:", instanceInfo.CreatedAt.ToString("HH:mm:ss"));

                // MonoBehaviour link
                if (instanceInfo.Instance is MonoBehaviour monoBehaviour)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("GameObject:", GUILayout.Width(100));
                    EditorGUILayout.ObjectField(monoBehaviour.gameObject, typeof(GameObject), true);
                    EditorGUILayout.EndHorizontal();
                }

                // Dependencies
                if (instanceInfo.Dependencies is { Count: > 0 })
                {
                    EditorGUILayout.LabelField($"Dependencies ({instanceInfo.Dependencies.Count}):", EditorStyles.boldLabel);

                    foreach (var dep in instanceInfo.Dependencies)
                    {
                        EditorGUILayout.LabelField($"  → {dep.Name}", _dependencyStyle);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No dependencies", EditorStyles.miniLabel);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private Color GetLifetimeColor(Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    return new Color(0.5f, 1f, 0.5f); // Green
                case Lifetime.Transient:
                    return new Color(1f, 0.8f, 0.5f); // Orange
                case Lifetime.Scoped:
                    return new Color(0.5f, 0.8f, 1f); // Blue
                default:
                    return Color.white;
            }
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
            {
                Repaint();
            }
        }
    }
}
