using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TODController))]
public class TODControllerEditor : Editor
{
    private SerializedProperty m_sunRotationConfigProperty;
    private SerializedProperty m_sunTransform;

    private Transform m_transform;

    void Awake()
    {
        m_sunRotationConfigProperty = serializedObject.FindProperty("m_sunRotationConfig");
        m_sunTransform = serializedObject.FindProperty("m_lightTransform");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Light Transform");
        m_sunTransform.objectReferenceValue = EditorGUILayout.ObjectField(m_sunTransform.objectReferenceValue, typeof(Transform), true, GUILayout.ExpandWidth(true)) as Transform;

        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.PrefixLabel("太阳方向动画配置");
        m_sunRotationConfigProperty.objectReferenceValue = EditorGUILayout.ObjectField(m_sunRotationConfigProperty.objectReferenceValue, typeof(TODSunRotation), false);
        if (GUILayout.Button("打开编辑器"))
        {
            if (m_sunTransform.objectReferenceValue == null)
                EditorUtility.DisplayDialog("Error", "请先将TOD太阳光Transform赋值", "确认");
            else
            {
                TODSunRotation sunRotation = m_sunRotationConfigProperty.objectReferenceValue as TODSunRotation;
                if (sunRotation == null)
                {
                    string path = EditorUtility.SaveFilePanelInProject("选择SunRotation配置文件保存位置", "SunRotationConfig", "asset", "D");
                    sunRotation = ScriptableObject.CreateInstance<TODSunRotation>();
                    AssetDatabase.CreateAsset(sunRotation, path);
                    m_sunRotationConfigProperty.objectReferenceValue = sunRotation;
                }

                SunDirEditorWindow sunEditor = EditorWindow.GetWindow<SunDirEditorWindow>();
                sunEditor.Init(sunRotation, m_sunTransform.objectReferenceValue as Transform);
                sunEditor.titleContent = new GUIContent("Sun Editor");
                Vector2 fixedSize = new Vector2(700, 500);
                sunEditor.minSize = fixedSize;
                //sunDirEditor.maxSize = fixedSize;
                sunEditor.Show();
            }

        }
        GUILayout.EndHorizontal();
        if(EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
