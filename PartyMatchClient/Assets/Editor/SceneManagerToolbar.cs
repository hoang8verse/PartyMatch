#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using UnityEngine.UIElements;
using System;
using System.IO;

[InitializeOnLoad]
public static class SceneManagerToolbar
{
    private static ScriptableObject _toolbar;
    static string[] _scenePaths;
    static string[] _sceneNames = { "" };

    static SceneManagerToolbar()
    {
        EditorApplication.delayCall += () =>
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        };
    }


    private static void Update()
    {
        if (_toolbar == null)
        {
            Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;

            UnityEngine.Object[] toolbars = UnityEngine.Resources.FindObjectsOfTypeAll(editorAssembly.GetType("UnityEditor.Toolbar"));
            _toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
            if (_toolbar != null)
            {
                var mRoot = _toolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                var rawRoot = mRoot.GetValue(_toolbar);
                var visualRoot = rawRoot as VisualElement;
                RegisterCallback("ToolbarZoneRightAlign", OnGUI);

                void RegisterCallback(string root, Action cb)
                {
                    var toolbarZone = visualRoot.Q(root);
                    if (toolbarZone != null)
                    {
                        var parent = new VisualElement()
                        {
                            style = 
                            {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                        };
                        var container = new IMGUIContainer();
                        container.onGUIHandler += () =>
                        {
                            cb?.Invoke();
                        };
                        parent.Add(container);
                        toolbarZone.Add(parent);
                    }
                }
                if (_scenePaths == null || _scenePaths.Length != EditorBuildSettings.scenes.Length)
                {
                    List<string> scenePaths = new List<string>();
                    List<string> sceneNames = new List<string>();

                    foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                    {
                        if (scene.path == null || scene.path.StartsWith("Assets") == false)
                            continue;

                        string scenePath = Application.dataPath + scene.path.Substring(6);
                        scenePaths.Add(scenePath);
                        sceneNames.Add(Path.GetFileNameWithoutExtension(scenePath));
                    }

                    _scenePaths = scenePaths.ToArray();
                    _sceneNames = sceneNames.ToArray();
                }
            }
        }
    }
    private static void OnGUI()
    {
        using (new EditorGUI.DisabledScope(Application.isPlaying))
        {
            string sceneName = EditorSceneManager.GetActiveScene().name;
            int sceneIndex = -1;
            for (int i = 0; i < _sceneNames.Length; i++)
            {
                if (sceneName == _sceneNames[i])
                {
                    sceneIndex = i;
                    break;
                }
            }

            int newSceneIndex = EditorGUILayout.Popup(sceneIndex, _sceneNames, GUILayout.Width(200f));
            if (newSceneIndex != sceneIndex)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(_scenePaths[newSceneIndex], OpenSceneMode.Single);
                }
            }
            
        }
    }
}
#endif