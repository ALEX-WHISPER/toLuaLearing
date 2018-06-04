using UnityEngine;
using System.Collections;
using LuaInterface;
using System;
using System.IO;

//展示searchpath 使用，require 与 dofile 区别
public class ScriptsFromFile : MonoBehaviour 
{
    LuaState lua = null;
    private string strLog = "";    

	void Start () 
    {
#if UNITY_5 || UNITY_2017 || UNITY_2018		
        Application.logMessageReceived += Log;
#else
        Application.RegisterLogCallback(Log);
#endif         
        lua = new LuaState();                
        lua.Start();        

        string fullPath = Application.dataPath + "\\ToLua/Examples/02_ScriptsFromFile";
        lua.AddSearchPath(fullPath);    //  将上述路径添加至lua可搜索路径里
    }

    void Log(string msg, string stackTrace, LogType type)
    {
        strLog += msg;
        strLog += "\r\n";
    }

    void OnGUI()
    {
        //  DoFile()/Require(): 加载文件至虚拟机，并执行里面的代码

        GUI.Label(new Rect(100, Screen.height / 2 - 100, 600, 400), strLog);

        //  DoFile: 每次调用都会重新加载和执行
        if (GUI.Button(new Rect(50, 50, 120, 45), "DoFile"))
        {
            strLog = "";
            lua.DoFile("ScriptsFromFile.lua");
        }

        //  Require: 只执行一次，不重复加载
        else if (GUI.Button(new Rect(50, 150, 120, 45), "Require"))
        {
            strLog = "";            
            lua.Require("ScriptsFromFile");     //  推荐方案       
        }

        lua.Collect();
        lua.CheckTop();
    }

    void OnApplicationQuit()
    {
        lua.Dispose();
        lua = null;
#if UNITY_5 || UNITY_2017 || UNITY_2018	
        Application.logMessageReceived -= Log;
#else
        Application.RegisterLogCallback(null);
#endif 
    }
}
