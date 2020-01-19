using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceInfoView : MonoBehaviour
{
    Text m_deviceInfoText;
    DeviceInfoModel m_deviceInfoModel;

    void Start()
    {
        m_deviceInfoModel = MonitorManager.Instance.GetModel<DeviceInfoModel>();
        m_deviceInfoText = transform.Find("Text").GetComponent<Text>();

        m_deviceInfoText.text = GenDeviceInfo();

        RectTransform rt = GetComponent<RectTransform>();
        
    }

    string GenDeviceInfo()
    {
        StringBuilder strBuilder = new StringBuilder();

        strBuilder.AppendFormat("<color=#66D1F8>CPU</color> : {0} [{1}]\n", m_deviceInfoModel.CPUInfo, m_deviceInfoModel.CPUCore);
        strBuilder.AppendFormat("<color=#66D1F8>RAM</color> : {0} MB\n", m_deviceInfoModel.RAMSize);
        strBuilder.AppendFormat("<color=#66D1F8>Graphics API</color> : {0}\n", m_deviceInfoModel.GraphicAPI);
        strBuilder.AppendFormat("<color=#66D1F8>GPU</color> : {0}\n", m_deviceInfoModel.GPU);
        strBuilder.AppendFormat("<color=#66D1F8>VRAM</color> : {0} MB\n", m_deviceInfoModel.VRAM);
        strBuilder.AppendFormat("<color=#66D1F8>Shader Level</color> : {0}\n", m_deviceInfoModel.ShaderLevel);
        strBuilder.AppendFormat("<color=#66D1F8>Screen</color> : {0}x{1} @{2} Hz\n", m_deviceInfoModel.ScreenWidth, m_deviceInfoModel.ScreenHeight, m_deviceInfoModel.ScreenRefreshRate);
        strBuilder.AppendFormat("<color=#66D1F8>OS</color> : {0}", m_deviceInfoModel.OS);

        return strBuilder.ToString();
    }
}
