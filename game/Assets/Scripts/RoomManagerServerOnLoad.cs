using kcp2k;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManagerServerOnLoad : MonoBehaviour
{
    #region MEMBERS
        private const string portArg = "port=";
        private const string keyArg = "key=";
    #endregion

    NetworkRoomManagerExt manager;
    KcpTransport kcpTransport;

    private ushort port = 7000;
    private string key = "";

    void Awake()
    {
        manager = GetComponent<NetworkRoomManagerExt>();
        kcpTransport = GetComponent<KcpTransport>();

        #if UNITY_SERVER
            ParseCommandLineArguments();
            HostRoom(); 
        #endif
    }


    // command line arguments to determine the port and room key
    private void ParseCommandLineArguments()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        string argumentString;

        for (int i = 0; i < args.Length; i++)
        {
            argumentString = "";
            if (args[i].StartsWith(portArg))
            {
                argumentString = args[i].Replace(portArg, "");
                port = Convert.ToUInt16(argumentString);
            }
            else if (args[i].StartsWith(keyArg))
            {
                key = args[i].Replace(keyArg, "");
            }
        }
    }

    // method only for server to start hosting room
    private void HostRoom()
    {
        kcpTransport.Port = port;
        manager.StartServer();
    }

}
