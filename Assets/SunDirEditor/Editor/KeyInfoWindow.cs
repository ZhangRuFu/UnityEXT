using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class KeyInfoWindow : EditorWindow {

    private float m_time;
    private Vector3 m_euler;
    private TODSunRotation.KeyFrame m_editKeyFrame;
    private Transform m_sunTransform;

    public delegate void KeyEditFinish(float time, Vector3 euler, TODSunRotation.KeyFrame editKeyFrame);
    private KeyEditFinish m_editFinish;

    public void SetPreInfo(TODSunRotation.KeyFrame keyFrame, KeyEditFinish editFinish, Transform sunTransform)
    {
        m_time = keyFrame.Time;
        m_euler = keyFrame.EulerRotation;
        m_editKeyFrame = keyFrame;
        m_editFinish = editFinish;
        m_sunTransform = sunTransform;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Time:   ");
        m_time = EditorGUILayout.FloatField(m_time);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        GUILayout.Label("Rotation:");
        m_euler = EditorGUILayout.Vector3Field("", m_euler);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        if(GUILayout.Button("修改为当前太阳旋转", GUILayout.ExpandHeight(true)))
            m_euler = m_sunTransform.localEulerAngles;
        if (GUILayout.Button("确认修改", GUILayout.ExpandHeight(true)))
        {
            m_editFinish(m_time, m_euler, m_editKeyFrame);
            Close();
        }
        GUILayout.EndVertical();
    }
}
