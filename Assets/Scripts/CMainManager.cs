using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Networking;
using System.IO.Ports;
using MiniJSON;
using System;

public class CMainManager : MonoBehaviour {

	private static CMainManager _instance;
	public static CMainManager Instance { get { return _instance; } }

	public AudioSource _audioSource;
	private SerialPort m_SerialPort;

	private void Awake()
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
	}
    void Start () 
	{
		//DiscoverLights();
		try
		{
			m_SerialPort = new SerialPort(CConfigMng.Instance._strSerialPort, 9600, Parity.None, 8, StopBits.One);
			m_SerialPort.Open();
			SerialPortWrite("1");
		}
		catch (Exception e)
		{
			Debug.Log(e);
			Debug.Log("Arduino : USB PORT ERROR");
		}
		List<Dictionary<string, object>> data = CSVReader.Read(CConfigMng.Instance._strExcelFileName);
		
		StartCoroutine(AudioPlayer(CConfigMng.Instance._strAudioFilePath));
		TimeLine();
	}
    private void Update()
    {
        #region test KeyBoard Input
        if (Input.GetKeyDown(KeyCode.A))
        {
			ZoneContent(m_nRightHue);
			ZoneContent(m_nRight_D_Hue);
			ZoneContent(m_nLeftHue);
			ZoneContent(m_nLeft_A_Hue);
			ZoneContent(m_nLeft_B_Hue);
		}
    }

    //존별 호출
    void ZoneContent(short[] nZoneHue)
    {
		for(short i = 0; i < nZoneHue.Length; i++)
        {
			transform.GetChild(nZoneHue[i]).GetComponentInChildren<CHueLamp>().HueSetup(new Color(UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), 100));
		}
    }
	#endregion

	#region Excel_TimeLine

	string[] m_strRight_Hues;
	string[] m_strRight_D_Hues;
	string[] m_strLeft_Hues;
	string[] m_strLeft_A_Hues;
	string[] m_strLeft_B_Hues;

	string m_strT5_A;
	string m_strT5_B;
	string m_strT5_C;

	private Color m_Color_Right;
	private Color m_Color_Right_D;
	private Color m_Color_Left;
	private Color m_Color_Left_A;
	private Color m_Color_Left_B;

	private float m_fNextTime = 0.0f;
	private short m_nCurrentNum = 0;

	private short[] m_nRightHue = { 0, 7, 8, 13 };
	private short[] m_nRight_D_Hue = { 1, 4 };
	private short[] m_nLeftHue = { 9, 10, 11 };
	private short[] m_nLeft_A_Hue = { 3, 5 };
	private short[] m_nLeft_B_Hue = { 2, 6 };

	private void TimeLine() 
    {
        List<Dictionary<string, object>> data = CSVReader.Read(CConfigMng.Instance._strExcelFileName);
		Debug.Log("Index : " + m_nCurrentNum);
		Debug.Log("현재 : " + (int)data[m_nCurrentNum]["TimeLoop"]);

		

		//Right_Color-----------------------------------------------------------------
		m_strRight_Hues = data[m_nCurrentNum]["Right_Hue"].ToString().Split('/');
		m_Color_Right = new Color(int.Parse(m_strRight_Hues[0]), int.Parse(m_strRight_Hues[1]), int.Parse(m_strRight_Hues[2]), int.Parse(m_strRight_Hues[3]) / 100);
		Debug.Log(m_Color_Right);

		//-----------------------------------------------------------------------------

		//Right_Color_D-----------------------------------------------------------------
		m_strRight_D_Hues = data[m_nCurrentNum]["Right_D_Hue"].ToString().Split('/');
		m_Color_Right_D = new Color(int.Parse(m_strRight_D_Hues[0]), int.Parse(m_strRight_D_Hues[1]), int.Parse(m_strRight_D_Hues[2]), int.Parse(m_strRight_D_Hues[3]) / 100);
		Debug.Log(m_Color_Right_D);

		//-----------------------------------------------------------------------------

		//Left_Color-----------------------------------------------------------------
		m_strLeft_Hues = data[m_nCurrentNum]["Left_Hue"].ToString().Split('/');
		m_Color_Left = new Color(int.Parse(m_strLeft_Hues[0]), int.Parse(m_strLeft_Hues[1]), int.Parse(m_strLeft_Hues[2]), int.Parse(m_strLeft_Hues[3]) / 100);
		Debug.Log(m_Color_Left);

		//-----------------------------------------------------------------------------

		//Left_A_Color-----------------------------------------------------------------
		m_strLeft_A_Hues = data[m_nCurrentNum]["Left_A_Hue"].ToString().Split('/');
		m_Color_Left_A = new Color(int.Parse(m_strLeft_A_Hues[0]), int.Parse(m_strLeft_A_Hues[1]), int.Parse(m_strLeft_A_Hues[2]), int.Parse(m_strLeft_A_Hues[3]));
		Debug.Log(m_Color_Left_A);

		//Left_B_Color-----------------------------------------------------------------
		m_strLeft_B_Hues = data[m_nCurrentNum]["Left_B_Hue"].ToString().Split('/');
		m_Color_Left_B = new Color(int.Parse(m_strLeft_B_Hues[0]), int.Parse(m_strLeft_B_Hues[1]), int.Parse(m_strLeft_B_Hues[2]), int.Parse(m_strLeft_B_Hues[3]) / 100);
		Debug.Log(m_Color_Left_B);

		//-----------------------------------------------------------------------------

		if (m_nCurrentNum == 0)
        {
            Invoke("TimeLine", (int)data[m_nCurrentNum]["TimeLoop"]);
            Debug.Log((int)data[m_nCurrentNum]["TimeLoop"] + "초 뒤 변환.");
        }
        else
        {
            m_fNextTime = (int)data[m_nCurrentNum + 1]["TimeLoop"] - (int)data[m_nCurrentNum]["TimeLoop"];
            Invoke("TimeLine", m_fNextTime);
            Debug.Log(m_fNextTime + "초 뒤 변환.");
        }
        m_nCurrentNum++;
        if (m_nCurrentNum >= data.Count - 1)
        {
            m_nCurrentNum = 0;

        }
        Debug.Log("다음 : " + (int)data[m_nCurrentNum]["TimeLoop"]);

		if(m_strT5_A != data[m_nCurrentNum]["T5_A"].ToString())
        {
			m_strT5_A = data[m_nCurrentNum]["T5_A"].ToString();
			//시리얼 포트 진행
			SerialPortWrite(m_strT5_A);

		}
		if (m_strT5_B != data[m_nCurrentNum]["T5_B"].ToString())
		{
			m_strT5_B = data[m_nCurrentNum]["T5_B"].ToString();
			//시리얼 포트 진행
			SerialPortWrite(m_strT5_B);

		}
		if (m_strT5_C != data[m_nCurrentNum]["T5_C"].ToString())
		{
			m_strT5_C = data[m_nCurrentNum]["T5_C"].ToString();
			//시리얼 포트 진행
			SerialPortWrite(m_strT5_C);
		}

	}
	void ZoneSoundContent(short[] nZoneHue, Color color)
	{
		for (short i = 0; i < nZoneHue.Length; i++)
		{
			transform.GetChild(nZoneHue[i]).GetComponentInChildren<CHueLamp>().HueSetup(color);
		}
	}
	#endregion

	#region AudioPlayer
	IEnumerator AudioPlayer(string filePath)
    {
		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(CConfigMng.Instance._strAudioFolderName + filePath, AudioType.MPEG))
		{
			yield return www.SendWebRequest();
			AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
			_audioSource.clip = audioClip;
			_audioSource.volume = CConfigMng.Instance._fAudioVolume;
			_audioSource.loop = true;
			_audioSource.Play();
		}
	}
    #endregion

    #region Hue_Group
    public bool on = true;

	/// <summary>
	/// 3 = Right, 7 = Left, 81 = In_Right, 82 = In_Left, 83 = In_Center
	/// </summary>
	/// <param name="strGroupId"></param>
	/// <param name="dataColor"></param>
	public void HueGroup(string strGroupId ,Color dataColor)
	{
		Debug.Log("호출");
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"http://{CConfigMng.Instance._HueIP}/api/{CConfigMng.Instance._HueDevice}/groups/{strGroupId}/action");
		request.Method = "PUT";

		Vector3 hsv = HSVFromRGB(dataColor);
		var state = new Dictionary<string, object>();
		state["on"] = on;
		state["hue"] = (int)(hsv.x / 360.0f * 65535.0f);
		state["sat"] = (int)(hsv.y * 255.0f);
		state["bri"] = (int)(hsv.z * 255.0f);

        if ((int)(hsv.z * 255.0f) == 0) state["on"] = false;

        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Json.Serialize(state));
		request.ContentLength = bytes.Length;

		System.IO.Stream s = request.GetRequestStream();
		s.Write(bytes, 0, bytes.Length);
		s.Close();

		HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		response.Close();
	}
	public static Vector3 HSVFromRGB(Color rgb)
	{
		float max = Mathf.Max(rgb.r, Mathf.Max(rgb.g, rgb.b));
		float min = Mathf.Min(rgb.r, Mathf.Min(rgb.g, rgb.b));
		float brightness = rgb.a;

		float hue, saturation;
		if (max == min)
		{
			hue = 0f;
			saturation = 0f;
		}
		else
		{
			float c = max - min;
			if (max == rgb.r)
			{
				hue = (rgb.g - rgb.b) / c;
			}
			else if (max == rgb.g)
			{
				hue = (rgb.b - rgb.r) / c + 2f;
			}
			else
			{
				hue = (rgb.r - rgb.g) / c + 4f;
			}
			hue *= 60f;
			if (hue < 0f)
			{
				hue += 360f;
			}
			saturation = c / max;
		}
		return new Vector3(hue, saturation, brightness);
	}
    #endregion

    #region Zone
    //HUE 개별 객체 생성
    public void DiscoverLights() 
	{
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + CConfigMng.Instance._HueIP + "/api/" + CConfigMng.Instance._HueDevice + "/lights");
		HttpWebResponse response = (HttpWebResponse)request.GetResponse ();

		System.IO.Stream stream = response.GetResponseStream();
		System.IO.StreamReader streamReader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
		
		var lights = (Dictionary<string, object>)Json.Deserialize (streamReader.ReadToEnd());
		foreach (string key in lights.Keys) 
		{
			var light = (Dictionary<string, object>)lights[key];

			foreach (CHueLamp hueLamp in GetComponentsInChildren<CHueLamp>()) 
			{
				if (hueLamp.devicePath.Equals(key)) goto Found;
			}
			
			if (light["type"].Equals("Extended color light")) 
			{
				GameObject gameObject = new GameObject();
				gameObject.name = (string)light["name"];
				gameObject.transform.parent = transform;
				gameObject.AddComponent<CHueLamp>();
				CHueLamp lamp = gameObject.GetComponent<CHueLamp>();
				lamp.devicePath = key;
			}
		Found:
			;
		}
	}
	#endregion

	#region SerialPort
	/// <summary>
	/// 릴레이 1 = 꺼짐 , 2 = 켜짐
	/// </summary>
	/// <param name="message"></param>
	void SerialPortWrite(string message)
	{
		m_SerialPort.Write(message);
	}
	#endregion
}
