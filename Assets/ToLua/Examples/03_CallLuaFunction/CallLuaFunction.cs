using UnityEngine;
using System.Collections;
using LuaInterface;
using System;

public class CallLuaFunction : MonoBehaviour 
{
    private string script =
        @"  function luaFunc(num)                        
                return num + 1
            end

            test = {}
            test.luaFunc = luaFunc
        ";

    LuaFunction luaFunc = null;
    LuaState lua = null;
    string tips = null;
	
	void Start () 
    {
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        new LuaResLoader();
        lua = new LuaState();
        lua.Start();
        DelegateFactory.Init();        
        lua.DoString(script);

        //  获取并缓存一个lua函数，支持串式操作，如："test.luaFunc", 表示test表中的luaFunc函数
        luaFunc = lua.GetFunction("test.luaFunc");

        if (luaFunc != null)
        {
            //  LuaFunction.Invoke(): 有一个返回值的调用
            int num = luaFunc.Invoke<int, int>(123456);
            Debugger.Log("generic call return: {0}", num);

            //  expansion call 原始方式
            num = CallFunc();
            Debugger.Log("expansion call return: {0}", num);

            //  delegate call
            Func<int, int> Func = luaFunc.ToDelegate<Func<int, int>>();
            num = Func(123456);
            Debugger.Log("Delegate call return: {0}", num);
            
            //  LuaState.Invoke(): 临时调用一个luaFunction并返回一个值，此操作不缓存luaFunction, 适合使用频率低的函数调用
            num = lua.Invoke<int, int>("test.luaFunc", 123456, true);
            Debugger.Log("luastate call return: {0}", num);
        }

        lua.CheckTop();
	}

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

#if !TEST_GC
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300), tips);
    }
#endif

    void OnDestroy()
    {
        //  释放LuaFunction
        if (luaFunc != null)
        {
            luaFunc.Dispose();
            luaFunc = null;
        }

        //  释放LuaState
        lua.Dispose();
        lua = null;

#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
    }

    int CallFunc()
    {
        luaFunc.BeginPCall();   //  开始函数调用
        luaFunc.Push(123456);   //  压入所需参数
        luaFunc.PCall();        //  调用
        int num = (int)luaFunc.CheckNumber();   //  数据类型保护
        luaFunc.EndPCall();     //  结束函数调用
        return num;                
    }
}
