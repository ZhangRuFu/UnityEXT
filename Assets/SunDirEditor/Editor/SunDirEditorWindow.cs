using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SunDirEditorWindow : EditorWindow
{
    class KeyMenuInfo
    {
        public float Time;
        public TODSunRotation.KeyFrame SelectKey;
    }

    TODSunRotation m_sunRotationConfig;
    TODSunRotation.KeyFrame m_curSelectKeyframe = null;

    Color m_fakeColor;
    Color lightGrey;
    Rect m_clientArea;

    Vector3 m_eulerAngle;
    float m_curTime;

    Transform m_sunTransform;

    Vector3 m_V3Start = Vector3.zero;
    Vector3 m_V3End = Vector3.zero;

    Vector2 m_keyFrameListScrollView = Vector2.zero;

    GUIStyle m_keyframe;
    GUIStyle m_boxBackground;

    //Menu
    GenericMenu m_keyFrameOperationMenu;

    public void Init(TODSunRotation sunConfig, Transform sunTransform)
    {
        m_sunRotationConfig = sunConfig;
        m_sunTransform = sunTransform;
    }

    private void Awake()
    {
        lightGrey = Color.white;
        lightGrey.a = 0.5f;

        m_keyframe = new GUIStyle("icon.keyframe");
        m_boxBackground = new GUIStyle("CapsuleButton");
    }

    private void OnAddKeySelected(object keyInfo)
    {
        KeyMenuInfo keyMenuInfo = keyInfo as KeyMenuInfo;
        Vector3 euler = m_sunRotationConfig.Evaluate(keyMenuInfo.Time);
        TODSunRotation.KeyFrame keyFrame = new TODSunRotation.KeyFrame(keyMenuInfo.Time, euler);
        m_sunRotationConfig.Add(keyFrame);
    }

    private void OnRemoveKeySelected(object keyInfo)
    {
        KeyMenuInfo keyMenuInfo = keyInfo as KeyMenuInfo;
        m_sunRotationConfig.Remove(keyMenuInfo.SelectKey);
    }

    private void OnEditKeySelected(object keyInfo)
    {
        KeyMenuInfo keyMenuInfo = keyInfo as KeyMenuInfo;
        KeyInfoWindow keyWindow = EditorWindow.GetWindow<KeyInfoWindow>();
        keyWindow.SetPreInfo(keyMenuInfo.SelectKey, OnEditKeyFinish, m_sunTransform.transform);
        Vector2 fixedSize = new Vector2(300, 150);
        keyWindow.titleContent = new GUIContent("KeyInfo");
        keyWindow.minSize = fixedSize;
        keyWindow.maxSize = fixedSize;
        keyWindow.Show();
    }

    private void OnEditKeyFinish(float time, Vector3 euler, TODSunRotation.KeyFrame keyFrame)
    {
        keyFrame.Time = time;
        keyFrame.EulerRotation = euler;
        m_sunRotationConfig.ResortKeyFrame(keyFrame);

        Focus();
    }

    private void OnGUI()
    {
        m_clientArea = position;
        m_clientArea.x = 20;
        m_clientArea.width -= 40;
        m_clientArea.y = 10;
        m_clientArea.height -= 10;
        Handles.BeginGUI();
        GUILayout.BeginArea(m_clientArea);
        
        //Time Line
        Rect timeLineRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(30));
        timeLineRect.width -= 1;

        float stepWidth = timeLineRect.width / 24;
        float smallStepWidth = stepWidth / 6;
        float hourStepWidth = timeLineRect.width / 8;

        Rect hourLabelRect = new Rect(timeLineRect.xMin, timeLineRect.yMin, 37, 20);
        for(int i = 0; i < 8; ++i)
        {
            hourLabelRect.x = timeLineRect.x + i * hourStepWidth;
            GUI.Label(hourLabelRect, string.Format("{0}:00", i * 3));
        }
        hourLabelRect.x = timeLineRect.x + 8 * hourStepWidth - hourLabelRect.width;
        GUI.Label(hourLabelRect, string.Format("{0}:00", 8 * 3));

        
        for (int i = 0; i <= 24; ++i)
        {
            float timeLineX = timeLineRect.x + stepWidth * i;
            float timeLineYStart = timeLineRect.yMax;
            float timeLineYEnd = timeLineYStart - 10;

            DrawLine(timeLineX, timeLineYStart, timeLineX, timeLineYEnd);
            if (i != 24)
            {
                for (int j = 1; j <= 5; ++j)
                {
                    float smallTimeLineX = timeLineX + j * smallStepWidth;
                    float smallTimeLineYStart = timeLineYStart;
                    float smallTimeLintYEnd = smallTimeLineYStart; ;
                    if (j == 3)
                        smallTimeLintYEnd -= 7;
                    else
                        smallTimeLintYEnd -= 5;

                    m_fakeColor = Handles.color;
                    Handles.color = Color.grey;
                    DrawLine(smallTimeLineX, smallTimeLineYStart, smallTimeLineX, smallTimeLintYEnd);
                    Handles.color = m_fakeColor;
                }
            }
        }

        Rect boxRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(30));
        boxRect.width -= 1;
        boxRect.y -= 1;

        Handles.DrawSolidRectangleWithOutline(boxRect, lightGrey, Color.white);

        DrawKeyframe(boxRect);

        //Pointer
        float pointerX = TimeToPositionX(m_curTime, boxRect.width) + boxRect.xMin;
        m_fakeColor = Handles.color;
        Handles.color = Color.red;
        DrawLine(pointerX, boxRect.yMax, pointerX, boxRect.yMax - 15);
        Handles.color = m_fakeColor;

        EditorGUI.BeginChangeCheck();
        m_curTime = GUILayout.HorizontalSlider(m_curTime, 0, 1, GUILayout.ExpandWidth(true));
        if(EditorGUI.EndChangeCheck())
            ControlSunGameObject(m_curTime);

        GUILayout.Space(20);
        
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        GUILayout.Label("Sun Rotation:");
        m_eulerAngle = EditorGUILayout.Vector3Field("", m_eulerAngle, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.Label("Time: " + Time01ToTime24(m_curTime));
        GUILayout.EndHorizontal();

        if(GUILayout.Button("生成当前太阳灯光方向为关键帧", GUILayout.ExpandWidth(true)))
        {
            if (m_sunTransform == null)
                ShowNotification(new GUIContent("Light对象未赋值"));
            else
            {
                TODSunRotation.KeyFrame keyFrame = new TODSunRotation.KeyFrame(m_curTime, m_sunTransform.transform.localEulerAngles);
                m_sunRotationConfig.Add(keyFrame);
            }
        }
        GUILayout.Space(20);

        //Key Frame List
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        GUILayout.Box("Index", m_boxBackground, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.Box("Rotation", m_boxBackground, GUILayout.Width(300), GUILayout.Height(20));
        GUILayout.Box("Time", m_boxBackground, GUILayout.ExpandWidth(true), GUILayout.Height(20));
        GUILayout.EndHorizontal();
        m_keyFrameListScrollView = GUILayout.BeginScrollView(m_keyFrameListScrollView, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        for (int i = 0; i < m_sunRotationConfig.KeyFrames.Count; ++i)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(i.ToString(), GUILayout.Width(200));
            GUILayout.Label(m_sunRotationConfig.KeyFrames[i].EulerRotation.ToString(), GUILayout.Width(300));
            GUILayout.Label(m_sunRotationConfig.KeyFrames[i].Time.ToString(), GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        
        GUILayout.EndArea();
        
        Handles.EndGUI();
    }

    private void DrawLine(float startX, float startY, float endX, float endY)
    {
        m_V3Start.x = startX;
        m_V3Start.y = startY;
        m_V3End.x = endX;
        m_V3End.y = endY;
        Handles.DrawLine(m_V3Start, m_V3End);
    }

    private float PositionToTime(Vector2 mousePosition, Rect area)
    {
        return (mousePosition.x - area.xMin) / area.width;
    }

    private float TimeToPositionX(float time, float width)
    {
        return time * width;
    }

    private string Time01ToTime24(float time)
    {
        float time24 = time * 24;
        int hour = (int)time24;
        int minutes = (int)((time24 - hour) * 60);
        return string.Format("{0}:{1}", hour, minutes > 10 ? minutes.ToString() : ("0" + minutes));
    }

    private void DrawKeyframe(Rect area)
    {
        switch(Event.current.type)
        {
            case EventType.MouseDown:
                {
                    if (!area.Contains(Event.current.mousePosition))
                        break;
                    float time = PositionToTime(Event.current.mousePosition, area);
                    m_curTime = time;

                    float diff = 8 / area.width;
                    TODSunRotation.KeyFrame selectKeyFrame = m_sunRotationConfig.SelectKeyframe(time, diff);

                    if (Event.current.button == 0)
                        m_curSelectKeyframe = selectKeyFrame;
                    else if (Event.current.button == 1)
                    {
                        KeyMenuInfo keyInfo = new KeyMenuInfo();
                        keyInfo.Time = time;
                        keyInfo.SelectKey = selectKeyFrame;

                        GenerateMenu(selectKeyFrame != null, keyInfo);
                        

                        m_keyFrameOperationMenu.ShowAsContext();
                    }

                    Event.current.Use();
                }
                    break;
            case EventType.MouseDrag:
                {
                    if (m_curSelectKeyframe == null)
                        break;

                    float time = Mathf.Clamp01(PositionToTime(Event.current.mousePosition, area));
                    m_curSelectKeyframe.Time = time;
                    
                    Event.current.Use();
                }
                break;
            case EventType.MouseUp:
                {
                    if (m_curSelectKeyframe == null)
                        break;

                    m_sunRotationConfig.ResortKeyFrame(m_curSelectKeyframe);
                    m_curSelectKeyframe = null;
                }
                break;
        }

        //Draw All Keyframe
        Rect rectPosition = new Rect();
        rectPosition.width = m_keyframe.fixedWidth;
        rectPosition.height = m_keyframe.fixedHeight;
        for(int i = 0; i < m_sunRotationConfig.KeyFrames.Count; ++i)
        {
            rectPosition.x = area.x + m_sunRotationConfig.KeyFrames[i].Time * area.width - rectPosition.width / 2;
            rectPosition.y = area.y + area.height / 2 - rectPosition.height / 2;

            if(m_sunRotationConfig.KeyFrames[i] == m_curSelectKeyframe)
            {
                m_fakeColor = GUI.color;
                GUI.color = Color.green;
            }

            GUI.Button(rectPosition, "", m_keyframe);

            if (m_sunRotationConfig.KeyFrames[i] == m_curSelectKeyframe)
                GUI.color = m_fakeColor;
        }

        //Check
        if (m_curSelectKeyframe != null)
            return;
        for (int i = 0; i < m_sunRotationConfig.KeyFrames.Count - 1; ++i)
            if (m_sunRotationConfig.KeyFrames[i].Time > m_sunRotationConfig.KeyFrames[i + 1].Time)
                Debug.LogError("帧顺序出错!-联系引擎解决");
    }

    private void ControlSunGameObject(float time)
    {
        if (m_sunTransform == null)
            return;

        if(m_sunRotationConfig.KeyFrames.Count == 0)
            return;

        m_eulerAngle = m_sunRotationConfig.Evaluate(time);
        m_sunTransform.transform.localEulerAngles = m_eulerAngle;
    }

    private void GenerateMenu(bool hasSelect, KeyMenuInfo keyInfo)
    {
        m_keyFrameOperationMenu = new GenericMenu();
        if(!hasSelect)
            m_keyFrameOperationMenu.AddItem(new GUIContent("Add Key"), false, OnAddKeySelected, keyInfo);
        else
            m_keyFrameOperationMenu.AddDisabledItem(new GUIContent("Add Key"));
        if (hasSelect)
        {
            m_keyFrameOperationMenu.AddItem(new GUIContent("Remove Key"), false, OnRemoveKeySelected, keyInfo);
            m_keyFrameOperationMenu.AddItem(new GUIContent("Edit Key"), false, OnEditKeySelected, keyInfo);
        }
        else
        {
            m_keyFrameOperationMenu.AddDisabledItem(new GUIContent("Remove Key"));
            m_keyFrameOperationMenu.AddDisabledItem(new GUIContent("Edit Key"));
        }
    }
}
