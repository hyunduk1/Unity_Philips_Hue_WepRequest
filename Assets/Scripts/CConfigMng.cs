using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.IO;


public class CConfigMng : MonoBehaviour
{
    private static CConfigMng _instance;
    public static CConfigMng Instance { get { return _instance; } }


    private static string strPath;
   
    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

    
    private string m_HueIP; public string _HueIP { get { return m_HueIP; } set { m_HueIP = value; } }
    private string m_HueDevice; public string _HueDevice { get { return m_HueDevice; } set { m_HueDevice = value; } }
    private string m_strSerialPort; public string _strSerialPort { get { return m_strSerialPort; } set { m_strSerialPort = value; } }
    private string m_strExcelFileName; public string _strExcelFileName { get { return m_strExcelFileName; } set { m_strExcelFileName = value; } }
    private float m_fHueDelayTime; public float _fHueDelayTime { get { return m_fHueDelayTime; } set { m_fHueDelayTime = value; } }
    private string m_strAudioFolderName; public string _strAudioFolderName { get { return m_strAudioFolderName; } set { m_strAudioFolderName = value; } }
    private string m_strAudioFilePath; public string _strAudioFilePath { get { return m_strAudioFilePath; } set { m_strAudioFilePath = value; } }
    private float m_fAudioVolume; public float _fAudioVolume { get { return m_fAudioVolume; } set { m_fAudioVolume = value; } }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        strPath = Application.dataPath + "/StreamingAssets/Config.ini";
        
        m_HueIP = IniReadValue("SET_VALUE", "HUE_IP");
        m_HueDevice = IniReadValue("SET_VALUE", "HUE_DEVICE");
        m_strSerialPort = IniReadValue("SET_VALUE", "SERIAL_PORT");
        m_strExcelFileName = IniReadValue("SET_VALUE", "EXCEL_FILE_NAME");
        m_fHueDelayTime = IniReadValueFloat("SET_VALUE", "HUE_DELAY_TIME");
        m_strAudioFolderName = IniReadValue("SET_VALUE", "AUDIO_FOLDER_NAME");
        m_strAudioFilePath = IniReadValue("SET_VALUE", "AUDIO_FILE_NAME");
        m_fAudioVolume = IniReadValueFloat("SET_VALUE", "AUDIO_VOLUME");
    }
       

    public static void IniWriteValue(string Section, string Key, string Value)
    {
        WritePrivateProfileString(Section, Key, Value, strPath);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public static string IniReadValue(string Section, string Key)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strPath);
        return temp.ToString();
    }


    public static float IniReadValueFloat(string Section, string Key)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strPath);
        float result = 0.0f;
        float.TryParse(temp.ToString(), out result);
        return result;
    }

    public static bool IniReadValuebool(string Section, string Key)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strPath);
        int result = 0;
        int.TryParse(temp.ToString(), out result);
        if (result == 1)
        {
            return true;
        }
        return false;
    }

    public static int IniReadValueInt(string Section, string Key)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strPath);
        int result = 0;
        int.TryParse(temp.ToString(), out result);
        return result;
    }


    public static int IniReadValueIntTimeData(string Section, string Key, string strDataPath)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strDataPath);
        int result = 0;
        int.TryParse(temp.ToString(), out result);
        return result;
    }
}
