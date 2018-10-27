using System;
using System.Collections.Generic;
using System.Reflection;

namespace HotFix_Project
{
    // TODO 蓝大.
    public class Ron<T>
    {
        public Ron(T arg)
        {
            UnityEngine.Debug.Log("Arg is ===>" + arg);
        }
    }

    public class InstanceClass
    {
        private int id;

        public InstanceClass()
        {
            UnityEngine.Debug.Log("!!! InstanceClass::InstanceClass()");
            this.id = 0;
        }

        public InstanceClass(int id)
        {
            UnityEngine.Debug.Log("!!! InstanceClass::InstanceClass() id = " + id);
            this.id = id;
        }

        public int ID
        {
            get { return id; }
        }

        // static method
        public static void StaticFunTest()
        {
            UnityEngine.Debug.Log("!!! InstanceClass.StaticFunTest()");
            
            // TODO 蓝大 测试代码
            Type t = typeof(Ron<>);
            UnityEngine.Debug.Log("t Type ==> " + t);
            Type constructed = t.MakeGenericType(new Type[]{typeof(string)});
            object obj = Activator.CreateInstance(constructed, new object[]{"Test Success"});

        }

        public static void StaticFunTest2(int a)
        {
            UnityEngine.Debug.Log("!!! InstanceClass.StaticFunTest2(), a=" + a);
        }

        public static void GenericMethod<T>(T a)
        {
            UnityEngine.Debug.Log("!!! InstanceClass.GenericMethod(), a=" + a);
        }
    }


}
