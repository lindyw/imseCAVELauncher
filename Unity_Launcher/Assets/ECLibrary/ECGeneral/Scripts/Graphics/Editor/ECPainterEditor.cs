using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ECPainter))]
public class ECPainterEditor : Editor {
    ECPainter editor;
    string directory = "Paints";
    string fileName = "paint";
    string extension = "png";
    void Awake () {
        editor = (ECPainter)target;
	}
	
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Saving Path"))
        {
            string path = EditorUtility.OpenFolderPanel("Please select a folder", directory, "");
            if (path.Length > 0)
            {
                ECFile store = new ECFile(path);
                directory = store.directory;
                fileName = store.file;
                extension = store.extension;
            }
            EditorUtility.SetDirty(editor);
        }
        GUILayout.EndHorizontal();
        directory = EditorGUILayout.TextField("Path: ", directory);
        fileName = EditorGUILayout.TextField("Naming: ", fileName);
        extension = EditorGUILayout.TextField("Extension: ", extension);
        DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Background"))
            {
                string path = EditorUtility.OpenFilePanel("Please pick a file", directory, extension);
                editor.bg = ECFile.LoadImage(path);
            }
            if (GUILayout.Button("Save"))
            {
                editor.Save(ECFile.Path(directory, fileName, extension));
            }
        }
        GUILayout.EndHorizontal();
        //GUILayout.BeginHorizontal();
        //data = EditorGUILayout.TextField("Data", data);
        //EditorGUILayout.LabelField("", value);
        //GUILayout.EndHorizontal();
        if (ECFile.Path(directory, fileName, extension) != editor.path) editor.path = ECFile.Path(directory, fileName, extension);
    }
}
