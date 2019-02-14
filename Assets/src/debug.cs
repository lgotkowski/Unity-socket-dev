using System;
using UnityEngine;

namespace src
{
    public class debug: MonoBehaviour
    {
        public void Start()
        {
//            Int64 int64 = 200;
//            byte[] int64Bytes = BitConverter.GetBytes(int64);
//            Debug.Log($"Int64 size: {int64Bytes.Length}");
//            
//            Int32 int32 = 200;
//            byte[] int32Bytes = BitConverter.GetBytes(int32);
//            Debug.Log($"Int32 size: {int32Bytes.Length}");
//            
//            Int16 int16 = 200;
//            byte[] int16Bytes = BitConverter.GetBytes(int16);
//            Debug.Log($"Int16 size: {int16Bytes.Length}");
//            
//            int Int = 200;
//            byte[] intBytes = BitConverter.GetBytes(Int);
//            Debug.Log($"Int size: {intBytes.Length}");

            string outMsgString = "Hello World!";
            Int64 outMsgLen = outMsgString.Length;
            byte[] outMsgBytes = System.Text.Encoding.ASCII.GetBytes(outMsgString);
            byte[] outHeader = BitConverter.GetBytes(outMsgLen);
            
            
            byte[] outBytes = new byte[outHeader.Length + outMsgBytes.Length];
            Array.Copy(outHeader, 0, outBytes, 0, outHeader.Length);
            Array.Copy(outMsgBytes, 0, outBytes, outHeader.Length, outMsgBytes.Length);

            /////////

            byte[] inBytes = outBytes;
            byte[] inHeader = new byte[8];
            Array.Copy(inBytes, 0, inHeader, 0, 8);
            Int64 inMsgLen = BitConverter.ToInt64(inHeader, 0);
            
            byte[] inMsgBytes = new byte[inMsgLen];
            Array.Copy(inBytes, 8, inMsgBytes, 0, inMsgLen);
            string inMsg = System.Text.Encoding.ASCII.GetString(inMsgBytes);
            Debug.Log($"InMsg: {inMsg}");
            
            // OLD //
            
            
//            string msgString = "Hello World!";
//            Int64 msgLen = msgString.Length;
//            Debug.Log($"msgLen: {msgLen}");
//            byte[] header = BitConverter.GetBytes(msgLen);
//
//            byte[] msgBytes = System.Text.Encoding.ASCII.GetBytes(msgString);
//            
//            byte[] outData = new byte[header.Length + msgBytes.Length];
//            Array.Copy(header, 0, outData, 0, header.Length);
//            Array.Copy(msgBytes, 0, outData, header.Length, msgBytes.Length);
//            
//            Debug.Log($"headerBytes: {header.Length}");
//            Debug.Log($"outData: {outData.Length}");
//            
//            byte[] inHeader = new byte[8];
//            Array.Copy(outData, 0, inHeader, 0, 8);
//            Int64 inMsgLen = BitConverter.ToInt64(inHeader, 0);
//            Debug.Log($"inMsgLen: {inMsgLen}");
//            
//            byte[] inData = new byte[inMsgLen];
//            Array.Copy(outData, 8, inData, 0, inMsgLen);
//            
//            Debug.Log($"inHeaderBytes: {inHeader.Length}");
//            Debug.Log($"inData: {inData.Length}");
//            
//            string inDataString = System.Text.Encoding.ASCII.GetString(inData);
//            Debug.Log($"In data string: {inDataString}");
        }
    }
}