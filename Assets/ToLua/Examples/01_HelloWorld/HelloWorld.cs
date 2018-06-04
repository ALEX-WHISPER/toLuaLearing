using UnityEngine;
using LuaInterface;
using System;

public class HelloWorld : MonoBehaviour
{
    void Awake()
    {
        //  启动luaState
        LuaState lua = new LuaState();  //  LuaState封装了对主要数据结构lua_State指针的各种堆栈操作. 一般推荐只创建一个LuaState对象.
                                        //  若要使用多个LuaState需要设置全局宏 MULTI_STATE
        lua.Start();

        string hello = "print('hello tolua#')"; //  lua代码字符串
        
        lua.DoString(hello, "_HelloWorld.cs");  //  将lua代码载入虚拟机中，由于无作用域，代码直接执行
        lua.CheckTop(); //  检测lua栈中是否还有未执行的指令
        lua.Dispose();  //  释放LuaState及其资源
        lua = null;     //  置空
    }
}
