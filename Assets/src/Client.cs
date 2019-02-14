using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net.Sockets;
using System.Threading;
using SimpleJSON;

using System.Linq;
using src;


public class Client : MonoBehaviour
{
    public Button m_BtnConnect;
    public Text m_Text;
    public string m_IpAdress = "127.0.0.1";
    public int m_Port = 65432;

    private TcpClient m_Client;
    private Thread m_ListenerThread;
    private Mutex m_Mutex = new Mutex();
    
    private ConcurrentQueue<JSONNode> m_Messages = new ConcurrentQueue<JSONNode>();
    
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        m_BtnConnect.onClick.AddListener(OnBtnConnectClicked);
    }

    void Update()
    {
        ReadMessageQueue();
    }
    
    void StartClient()
    {
        m_Client = new TcpClient();
        try
        {
            m_Client.Connect(m_IpAdress, m_Port);
            NetworkStream serverStream = m_Client.GetStream();
            Send(serverStream);
            StartListen(serverStream);
        }
        catch (Exception e)
        {
            Debug.Log("Could not connect to Server!");
            //throw;
        }
    }
    
    void Send(NetworkStream serverStream)
    {
        // Sending to server
        string msg = "Hello this is Unity.";
        JSONNode outDataDict = new JSONObject();
            
        outDataDict.Add("msg", msg);

        byte[] outStream = MessageHandler.CreateDataBytes(outDataDict);
        serverStream.Write(outStream, 0, outStream.Length);
        serverStream.Flush();
        Debug.Log("Sent message to server done.");
    }

    void StartListen(NetworkStream serverStream)
    {
        m_ListenerThread = new Thread(() =>Listen(serverStream)); // starting a thread with a function that takes parameters
        m_ListenerThread.IsBackground = true;
        m_ListenerThread.Start();
    }

    void Listen(NetworkStream serverStream)
    {
        Debug.Log("Listening....");
        // Reading from server
        //byte[] inStream = new byte[1024];
        
        
        byte[] inStreamHeader = new byte[8];
        serverStream.Read(inStreamHeader, 0, inStreamHeader.Length);
        Int64 header = BitConverter.ToInt64(inStreamHeader, 0);
        
        
        byte[] inStreamMessage = new byte[header];
        serverStream.Read(inStreamMessage, 0, inStreamMessage.Length);
        string dataString = System.Text.Encoding.ASCII.GetString(inStreamMessage);
        Debug.Log($"Data string: {dataString}");
        JSONNode dataDict = JSON.Parse(dataString);
        m_Messages.Enqueue(dataDict);   
        
        serverStream.Close();
        m_Client.Close();
        
        
        
        
//        List<byte> buffer = new List<byte>();
//        byte[] tmpBuffer = new byte[1024];
//
//        while (serverStream.Read(tmpBuffer, 0, tmpBuffer.Length) > 0)
//        {
//            buffer.AddRange(tmpBuffer);
//        }
//        byte[] inStream = new byte[buffer.Count];
//        buffer.CopyTo(inStream);
        
        
            
//        m_Mutex.WaitOne();
//        MessageHandler msgHandler = new MessageHandler();
//        List<JSONNode> inDataDict = msgHandler.CreateDataDict(inStream);
//        
//        // TODO: do not only take index 0 and also do not create the mshHandler here but probably on connect and keep it alive while listening
//        m_Messages.Enqueue(inDataDict[0]);            
//        m_Mutex.ReleaseMutex();
//
//        serverStream.Close();
//        m_Client.Close();
    }
    
    void OnApplicationQuit()
    {
        if (m_ListenerThread != null)
        {
            m_ListenerThread.Abort();
            Debug.Log("Killed Thread");
            Debug.Log($"Listener thread alive: {m_ListenerThread.IsAlive}");
        }
    }

    void OnBtnConnectClicked()
    {
        StartClient();
    }

    void ReadMessageQueue()
    {
        if(m_Messages.Count == 0) return;
        
        JSONNode dataDict;
        m_Messages.TryDequeue(out dataDict);
        string msg = dataDict["msg"];
        
        Debug.Log($"In Msg: {msg}");
        m_Text.text = msg;
    }
}
