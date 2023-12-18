using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Dialog
{
    public class DialogueEditor : EditorWindow
    {
        [MenuItem("Window/Dialogue")]
        public static DialogueEditor ShowWindow()
        {
            return GetWindow<DialogueEditor>();
        }

        [OnOpenAssetAttribute]
        public static bool OpenDialogAsset(int instanceId, int line)
        {
            var instance = EditorUtility.InstanceIDToObject(instanceId);

            if (instance is not Dialogue) return false;

            ShowWindow().selectedDialogue = (AssetDatabase.GetAssetPath(instance), instance as Dialogue);

            return true;
        }

        private List<(string path, Dialogue dialogue)> dialogueList = new List<(string path, Dialogue dialogue)>();

        private (string path, Dialogue dialogue) selectedDialogue;

        private Vector2 listScrollPos = Vector2.zero, linesScrollPos = Vector2.zero, choicesScrollPos = Vector2.zero;

        private SerializedObject serialized;

        private void Awake()
        {
            dialogueList = LoadAssets<Dialogue>("t:Dialogue");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            try
            {
                RenderDialogueList();

                EditorGUILayout.BeginVertical();
                try
                {
                    if (!selectedDialogue.dialogue)
                    {
                        GUILayout.Label("No dialogue selected");
                        return;
                    }

                    if (serialized == null)
                    {
                        serialized = new SerializedObject(selectedDialogue.dialogue);
                    }

                    GUILayout.Label(selectedDialogue.path, EditorStyles.whiteLargeLabel);

                    #region Lines

                    GUILayout.Label($"Lines: {selectedDialogue.dialogue.lines.Count}", EditorStyles.whiteLargeLabel);

                    linesScrollPos = EditorGUILayout.BeginScrollView(linesScrollPos);
                    try
                    {
                        foreach (var line in selectedDialogue.dialogue.lines)
                        {
                            if (DrawLineEditor(line, () =>
                            {
                                serialized.FindProperty("lines").DeleteArrayElementAtIndex(selectedDialogue.dialogue.lines.FindIndex(l => l == line));
                                selectedDialogue.dialogue.lines.Remove(line);
                                serialized.ApplyModifiedProperties();
                            }))
                            {
                                return;
                            }
                            EditorGUILayout.Space(10f);
                        }
                    }
                    finally
                    {
                        EditorGUILayout.EndScrollView();
                    }

                    AddButton("Add new line", () =>
                    {
                        selectedDialogue.dialogue.lines.Add(new Line());
                        serialized.FindProperty("lines").InsertArrayElementAtIndex(selectedDialogue.dialogue.lines.Count - 1);
                        serialized.ApplyModifiedProperties();
                    });

                    #endregion

                    #region Choices

                    GUILayout.Label($"Choices: {selectedDialogue.dialogue.choices.Count}", EditorStyles.whiteLargeLabel);

                    choicesScrollPos = EditorGUILayout.BeginScrollView(choicesScrollPos);
                    try
                    {
                        foreach (var choice in selectedDialogue.dialogue.choices)
                        {
                            EditorGUILayout.BeginHorizontal();

                            GUILayout.Label("Display");
                            choice.displayText = EditorGUILayout.TextField(choice.displayText);

                            EditorGUILayout.EndHorizontal();

                            if (DrawLineEditor(choice.line, () =>
                            {
                                serialized.FindProperty("choices").DeleteArrayElementAtIndex(selectedDialogue.dialogue.choices.FindIndex(c => c == choice));
                                selectedDialogue.dialogue.choices.Remove(choice);
                                serialized.ApplyModifiedProperties();
                            }))
                            {
                                return;
                            }

                            EditorGUILayout.BeginHorizontal();

                            GUILayout.Label("Response");
                            choice.responseDialog = EditorGUILayout.ObjectField("", choice.responseDialog, typeof(Dialogue), allowSceneObjects: false) as Dialogue;

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(10f);
                        }
                    }
                    finally
                    {
                        EditorGUILayout.EndScrollView();
                    }

                    EditorGUILayout.BeginHorizontal();

                    AddButton("Add new existing choice", () =>
                    {
                        EditorGUIUtility.ShowObjectPicker<Choice>(null, false, "t:Choice", 0);
                    });

                    if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == 0)
                    {
                        var selected = EditorGUIUtility.GetObjectPickerObject();
                        if (selected != null && selected is Choice)
                        {
                            selectedDialogue.dialogue.choices.Add(selected as Choice);

                            serialized.FindProperty("choices").InsertArrayElementAtIndex(selectedDialogue.dialogue.choices.Count - 1);
                            serialized.FindProperty("choices")
                                .GetArrayElementAtIndex(selectedDialogue.dialogue.choices.Count - 1)
                                .objectReferenceValue = selected;

                            serialized.ApplyModifiedProperties();
                        }
                    }

                    AddButton("Add new choice", () =>
                    {
                        var choice = CreateInstance<Choice>();
                        AssetDatabase.CreateAsset(choice, $"Assets/{selectedDialogue.dialogue.name}Choice_{selectedDialogue.dialogue.choices.Count}.asset");

                        selectedDialogue.dialogue.choices.Add(choice);

                        serialized.FindProperty("choices").InsertArrayElementAtIndex(selectedDialogue.dialogue.choices.Count - 1);
                        serialized.FindProperty("choices")
                            .GetArrayElementAtIndex(selectedDialogue.dialogue.choices.Count - 1)
                            .objectReferenceValue = choice;

                        serialized.ApplyModifiedProperties();
                    });

                    EditorGUILayout.EndHorizontal();

                    #endregion

                }
                finally
                {
                    EditorGUILayout.EndVertical();
                }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        private void OnDestroy()
        {
            Save();
        }

        private void Save()
        {
            if (serialized == null)
            {
                return;
            }
            var serializedLines = serialized.FindProperty("lines");

            for (int i = 0; i < selectedDialogue.dialogue.lines.Count; i++)
            {
                var line = serializedLines.GetArrayElementAtIndex(i);
                line.FindPropertyRelative("content").stringValue = selectedDialogue.dialogue.lines[i].content;
                line.FindPropertyRelative("speaker").objectReferenceValue =
                    selectedDialogue.dialogue.lines[i].speaker;
            }

            var serializedChoices = serialized.FindProperty("choices");

            for (int i = 0; i < selectedDialogue.dialogue.choices.Count; i++)
            {
                var choice = new SerializedObject(serializedChoices.GetArrayElementAtIndex(i).objectReferenceValue as Choice);

                var line = choice.FindProperty("line");
                line.FindPropertyRelative("content").stringValue = selectedDialogue.dialogue.choices[i].line.content;
                line.FindPropertyRelative("speaker").objectReferenceValue =
                    selectedDialogue.dialogue.choices[i].line.speaker;

                choice.FindProperty("displayText").stringValue = selectedDialogue.dialogue.choices[i].displayText;
                choice.FindProperty("responseDialog").objectReferenceValue =
                    selectedDialogue.dialogue.choices[i].responseDialog;

                EditorUtility.SetDirty(choice.targetObject);
                choice.ApplyModifiedProperties();
            }

            serialized.ApplyModifiedProperties();
        }

        /// <summary>
        /// Renders a list, where dialog items are listed as links
        /// </summary>
        private void RenderDialogueList()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(200));

            GUILayout.Label("Dialog items");

            EditorGUILayout.BeginScrollView(listScrollPos);

            foreach (var dialogListItem in dialogueList)
            {
                if (EditorGUILayout.LinkButton(dialogListItem.path))
                {
                    Save();
                    selectedDialogue = dialogListItem;
                }
            }

            EditorGUILayout.EndScrollView();

            AddButton("New", () =>
            {
                AssetDatabase.CreateAsset(CreateInstance<Dialogue>(), $"Assets/New Dialogue {dialogueList.Count}.asset");
                dialogueList = LoadAssets<Dialogue>("t:Dialogue");
            });

            AddButton("Fetch assets", () =>
            {
                dialogueList = LoadAssets<Dialogue>("t:Dialogue");
            });

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Add a button
        /// </summary>
        /// <param name="txt">Button text</param>
        /// <param name="callBack">Callback, when button is pressed</param>
        private void AddButton(string txt, Action callBack)
        {
            if (GUILayout.Button(txt))
            {
                callBack();
            }
        }

        /// <summary>
        /// Render an editor for a line
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="removeButtonAction">Callback, when the remove button is pressed</param>
        /// <returns></returns>
        private bool DrawLineEditor(Line line, Action removeButtonAction)
        {
            EditorGUILayout.BeginHorizontal("box");

            if (line.speaker?.portrait?.texture)
                GUILayout.Label(line.speaker.portrait.texture, GUILayout.Width(100), GUILayout.Height(100));

            line.speaker = EditorGUILayout.ObjectField("", line.speaker, typeof(Actor), allowSceneObjects: false) as Actor;

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                removeButtonAction();
                EditorGUILayout.EndHorizontal();
                return true;
            }

            EditorGUILayout.EndHorizontal();

            line.content = EditorGUILayout.TextArea(line.content, GUILayout.Height(100));
            return false;
        }

        /// <summary>
        /// Load assets of type 
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="filter">Filter by which to load</param>
        /// <returns>A list of paths and items</returns>
        private static List<(string path, T asset)> LoadAssets<T>(string filter) where T : UnityEngine.Object
        {
            var GUIDs = AssetDatabase.FindAssets(filter);
            List<(string path, T asset)> assets = new List<(string path, T asset)>();
            foreach (var GUID in GUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(GUID);
                assets.Add((path, AssetDatabase.LoadAssetAtPath<T>(path)));
            }
            return assets;
        }

    }
}
