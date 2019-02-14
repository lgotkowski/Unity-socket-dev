using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleJSON;
using UnityEngine;

namespace src
{
    public class MessageHandler
    {
        private byte[] m_Buffer;
        private int m_HeaderSize = 8;
        private Int64 m_DataSize = -1;
        private JSONNode m_DataDict;

        public static byte[] CreateDataBytes(JSONNode dataDict)
        {
            string dataString = dataDict.ToString();
            byte[] dataBytes = System.Text.Encoding.ASCII.GetBytes(dataString);
            
            Int64 header = dataString.Length;
            byte[] headerBytes = BitConverter.GetBytes(header);
            
            byte[] outBytes = new byte[headerBytes.Length + dataBytes.Length];
            Array.Copy(headerBytes, 0, outBytes, 0, headerBytes.Length);
            Array.Copy(dataBytes, 0, outBytes, headerBytes.Length, dataBytes.Length);
            
            return outBytes;
        }

        // TODO: string dataString probably needs to be a byte[] and input is probably the byte[] inStream of the  serverStream.Read(inStream, 0, (int)1024); 
        public List<JSONNode> CreateDataDict(byte[] inBytes)
        {
            List<JSONNode> data = new List<JSONNode>();
            if (m_Buffer != null)
            {
                // append inBytes to m_Buffer
                MemoryStream tmpStream = new MemoryStream();
                tmpStream.Write(m_Buffer, 0, m_Buffer.Length);
                tmpStream.Write(inBytes, 0, inBytes.Length);
                m_Buffer = tmpStream.ToArray();
            }
            else
            {
                m_Buffer = inBytes;
            }
            
            
            Debug.Log($"Len buffer: {m_Buffer.Length}");
            
            //Array.Copy(inBytes, 0, m_Buffer, m_Buffer.Length, inBytes.Length);
            
//            MemoryStream s = new MemoryStream();
//            s.Write(m_Buffer, 0, m_Buffer.Length);
//            s.Write(inStream, 0, inStream.Length);
//            m_Buffer = s.ToArray();
            
            while (m_Buffer.Length > m_HeaderSize)
            {
                if (m_DataSize < 0)
                {
                    m_DataSize = GetDataSize();
                    Debug.Log($"Data Size: {m_DataSize}");
                }

                if (m_DataDict == null)
                {
                    m_DataDict = GetDataDict();
                    Debug.Log($"Data Dict: {m_DataDict}");
                }

                if (m_DataDict)
                {
                    data.Append(m_DataDict);
                    m_DataSize = -1;
                    m_DataDict = null;
                }
            }
            return data;
        }

        Int64 GetDataSize()
        {
            if (m_Buffer.Length >= m_HeaderSize)
            {
                byte[] inHeader = new byte[m_HeaderSize];
                Array.Copy(m_Buffer, 0, inHeader, 0, m_HeaderSize);
                // header is the length of the acctual message
                Int64 header = BitConverter.ToInt64(inHeader, 0);
                
                // remove the header from the buffer so only keep everything after the header
                m_Buffer = TimBuffer(m_HeaderSize, m_Buffer.Length - m_HeaderSize);
                return header;
                
//                byte[] tmpBuffer = new byte[header];
//                Array.Copy(m_Buffer, m_HeaderSize, tmpBuffer, 0, header);
//                m_Buffer = tmpBuffer;
            }
            return -1;
        }

        JSONNode GetDataDict()
        {
            if (m_Buffer.Length >= m_DataSize)
            {
                byte[] dataBytes = new byte[m_DataSize];
                Array.Copy(m_Buffer, 0, dataBytes, 0, m_DataSize);
                
                // Remove the dataBytes from the buffer
                m_Buffer = TimBuffer(0, (int)m_DataSize);
                string dataString = System.Text.Encoding.ASCII.GetString(dataBytes);
                JSONNode dataDict = JSON.Parse(dataString);
                return dataDict;
                
                
//                byte[] dataBytes = new byte[m_DataSize];
//                Array.Copy(m_Buffer, 0, dataBytes, 0, m_DataSize);
//                string dataString = System.Text.Encoding.ASCII.GetString(dataBytes);
//                JSONNode dataDict = JSON.Parse(dataString);
//                return dataDict;
            }
            return null;
        }

        byte[] TimBuffer(int srcIndex, int length)
        {
            byte[] tmpBuffer = new byte[length];
            Array.Copy(m_Buffer, srcIndex, tmpBuffer, 0, length);
            return tmpBuffer;   
        }
    }
}