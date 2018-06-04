using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

public class AccessingLuaVariables : MonoBehaviour 
{
    private string script =
        @"
            print('Objs2Spawn is: '..Objs2Spawn)
            var2read = 42
            varTable = {1,2,3,4,5}
            varTable.default = 1
            varTable.map = {}
            varTable.map.name = 'map'
            
            meta = {name = 'meta'}
            setmetatable(varTable, meta)
            
            function TestFunc(strs)
                print('get func by variable')
            end
        ";

	void Start () 
    {
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        new LuaResLoader();
        LuaState lua = new LuaState();
        lua.Start();

        //  在lua中声明变量：Objs2Spawn, 并赋值为5
        lua["Objs2Spawn"] = 5;
        lua.DoString(script);

        //  通过LuaState访问
        Debugger.Log("Read var from lua: {0}", lua["var2read"]);
        Debugger.Log("Read table var from lua: {0}", lua["varTable.default"]);  //  LuaState 拆串式table

        //  调用lua方法
        LuaFunction func = lua["TestFunc"] as LuaFunction;
        func.Call();
        func.Dispose();

        //  cache成LuaTable进行访问
        LuaTable table = lua.GetTable("varTable");

        //  访问
        Debugger.Log("Read varTable from lua, default: {0} name: {1}", table["default"], table["map.name"]);
        
        //  修改
        table["map.name"] = "new1";  //  table 字符串只能是key
        Debugger.Log("Modify varTable name: {0}", table["map.name"]);

        //  添加表
        table.AddTable("newmap");
        LuaTable table1 = (LuaTable)table["newmap"];
        table1["name"] = "table1";
        Debugger.Log("varTable.newmap name: {0}", table1["name"]);
        table1.Dispose();

        //  获取元表
        table1 = table.GetMetaTable();

        if (table1 != null)
        {
            Debugger.Log("varTable metatable name: {0}", table1["name"]);
        }

        //   获取表中数组
        object[] list = table.ToArray();

        for (int i = 0; i < list.Length; i++)
        {
            Debugger.Log("varTable[{0}], is {1}", i, list[i]);
        }

        table.Dispose();
        lua.CheckTop();
        lua.Dispose();
	}

    private void OnApplicationQuit()
    {
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
    }

    string tips = null;

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, 600, 400), tips);
    }
}
