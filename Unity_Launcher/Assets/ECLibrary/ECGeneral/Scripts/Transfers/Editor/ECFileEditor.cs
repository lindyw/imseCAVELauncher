using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ECFile))]
public class ECFileEditor : Editor
{
    ECFile editor;
    MonoScript script;
    void Awake()
    {
    }
    void OnEnable()
    {
        editor = (ECFile)target;
        script = MonoScript.FromMonoBehaviour(editor);
    }
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;
        if (script != MonoScript.FromMonoBehaviour(editor)) { script = MonoScript.FromMonoBehaviour(editor); }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Directory"))
        {
            string path = EditorUtility.OpenFolderPanel("Please select a folder", editor.directory, "");
            if (path.Length > 0) { 
                editor.directory = path;
                string[] files = ECFile.FileCounts(editor.directory);
                foreach (string f in files) Debug.Log(ECFile.FileName(f) + "." + ECFile.Extension(f));
                Debug.Log(files.Length + " file(s) found.");
            }
            EditorUtility.SetDirty(editor);
        } 
        if (editor.FileExists())
        {
            if (GUILayout.Button("Explorer"))
            {
                string path = System.IO.Path.GetFullPath(editor.Path().Replace(@"/", @"\"));
                System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Path", EditorStyles.boldLabel);
        editor.directory = EditorGUILayout.TextField("Current Directory: ", editor.directory);
        editor.file = EditorGUILayout.TextField("Current File Name: ", editor.file);
        editor.extension = EditorGUILayout.TextField("Current Extension: ", editor.extension);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load File"))
        {
            string path = EditorUtility.OpenFilePanel("Please pick a file", editor.directory, editor.extension);
            if (path.Length > 0)
            {
                editor.directory = ECFile.DirectoryName(path);
                editor.file = ECFile.FileName(path);
                editor.extension = ECFile.Extension(path);
                editor.GetData();
                foreach (string d in editor.value) Debug.Log(d);
            }
            EditorUtility.SetDirty(editor);
        } 
        if (editor.FileExists())
        {
            if (GUILayout.Button("Save File"))
            {
                editor.SetData();
            }
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
        editor.separator = EditorGUILayout.TextField("Separator: ", editor.separator);
        editor.dataLength = EditorGUILayout.IntField("Number of data: ", editor.dataLength, EditorStyles.boldLabel);
        if (editor.data.Length != editor.dataLength)
        {
            editor.ResizeData();
        }
        for (int i = 0; i < editor.data.Length; i++)
        {
            editor.data[i] = EditorGUILayout.TextField(" - Data Name " + (i+1) + ": ", editor.data[i]);
        }

        if (editor.FileExists())
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear File"))
            {
                editor.ClearFile();
            }
            if (GUILayout.Button("Delete File"))
            {
                editor.DeleteFile();
            } 
            GUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Create File"))
            {
                if (!editor.DirectoryExists()) editor.CreateDirectory();
                editor.CreateFile();
            }
        }
        //GUILayout.BeginHorizontal();
        //data = EditorGUILayout.TextField("Data", data);
        //EditorGUILayout.LabelField("", value);
        //GUILayout.EndHorizontal();
    }
}
