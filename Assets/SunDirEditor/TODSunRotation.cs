using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TODSunRotation : ScriptableObject
{
    [System.Serializable]
    public class KeyFrame
    {
        public float Time;
        public Vector3 EulerRotation;

        public KeyFrame(float time, Vector3 eulerRotation)
        {
            Time = time;
            EulerRotation = eulerRotation;
        }
    }

    [SerializeField]
    private List<KeyFrame> m_keyFrames = new List<KeyFrame>();

    public List<KeyFrame> KeyFrames { get { return m_keyFrames; } }
    public void Add(KeyFrame keyFrame)
    {
        if (keyFrame == null)
            return;

        int finalIndex = -1;
        for (int i = 0; i < m_keyFrames.Count; ++i)
            if (m_keyFrames[i].Time >= keyFrame.Time)
            {
                finalIndex = i;
                break;
            }
        if (finalIndex == -1)
            finalIndex = m_keyFrames.Count;
        m_keyFrames.Insert(finalIndex, keyFrame);
    }
    public void Remove(KeyFrame keyFrame)
    {
        m_keyFrames.Remove(keyFrame);
    }

    public KeyFrame SelectKeyframe(float time, float diff)
    {
        for (int i = 0; i < m_keyFrames.Count; ++i)
            if (m_keyFrames[i].Time >= time - diff && m_keyFrames[i].Time <= time + diff)
                return m_keyFrames[i];
        return null;
    }

    public void ResortKeyFrame(KeyFrame needResortKeyFrame)
    {
        //Corect Position
        //TODO : Use Bin-Search Optimize
        int i = 0;
        for (i = 0; i < m_keyFrames.Count; ++i)
        {
            if (m_keyFrames[i] == needResortKeyFrame)
                continue;

            if (needResortKeyFrame.Time <= m_keyFrames[i].Time)
            {
                int index = m_keyFrames.IndexOf(needResortKeyFrame);
                if (index > i)
                {
                    for (int j = index; j > i; --j)
                        m_keyFrames[j] = m_keyFrames[j - 1];
                    m_keyFrames[i] = needResortKeyFrame;
                    break;

                }
                else
                {
                    for (int j = index; j < i - 1; ++j)
                        m_keyFrames[j] = m_keyFrames[j + 1];
                    m_keyFrames[i - 1] = needResortKeyFrame;
                    break;

                }
            }
        }

        if (i == m_keyFrames.Count && m_keyFrames[i - 1] != needResortKeyFrame)
        {
            int index = m_keyFrames.IndexOf(needResortKeyFrame);
            for (int j = index; j < i - 1; ++j)
                m_keyFrames[j] = m_keyFrames[j + 1];
            m_keyFrames[i - 1] = needResortKeyFrame;
        }
    }

    public Vector3 Evaluate(float time)
    {
        if (m_keyFrames.Count == 0)
            return Vector3.zero;

        int suitIndex = -1;
        for (int i = 0; i < m_keyFrames.Count; ++i)
            if (time <= m_keyFrames[i].Time)
            {
                suitIndex = i;
                break;
            }
        if (suitIndex == 0)
            return m_keyFrames[0].EulerRotation;
        else if (suitIndex == -1)
            return m_keyFrames[m_keyFrames.Count - 1].EulerRotation;
        else
        {
            int preIndex = suitIndex - 1;
            int postIndex = suitIndex;
            float t = (time - m_keyFrames[preIndex].Time) / (m_keyFrames[postIndex].Time - m_keyFrames[preIndex].Time);
            return Quaternion.Lerp(Quaternion.Euler(m_keyFrames[preIndex].EulerRotation), Quaternion.Euler(m_keyFrames[postIndex].EulerRotation), t).eulerAngles;
        }
    }
}
