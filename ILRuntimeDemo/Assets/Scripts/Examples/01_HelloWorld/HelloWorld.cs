using UnityEngine;
using System.Collections;
using System.IO;
using ILRuntime.Runtime.Enviorment;
using System.Reflection;

// 我的问题是这样的.我想在dll里通过反射创建带有泛型参数的实例
// TODO 蓝大 主工程里的测试代码.是正常的
public class Ron2<T>
{
    public Ron2(T arg)
    {
        UnityEngine.Debug.Log("Ron2 arg ==>" + arg);
    }
}


public class HelloWorld : MonoBehaviour
{
    //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
    //大家在正式项目中请全局只创建一个AppDomain
    AppDomain appdomain;

    void Start()
    {
        // TODO 蓝大, 主工程的测试代码
        System.Type t = typeof(Ron2<>);
        UnityEngine.Debug.Log("t Type ==> " + t);
        System.Type constructed = t.MakeGenericType(new System.Type[]{typeof(string)});
        object obj = System.Activator.CreateInstance(constructed, new object[]{"Test Success"});


        StartCoroutine(LoadHotFixAssembly());
    }

    IEnumerator LoadHotFixAssembly()
    {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
        //正式发布的时候需要大家自行从其他地方读取dll

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.dll");
#else
        WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.dll");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();

        //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
#if UNITY_ANDROID
        www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.pdb");
#else
        www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.pdb");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] pdb = www.bytes;

        System.IO.MemoryStream fs = new MemoryStream(dll);
        appdomain.LoadAssembly(fs, null, null);

        // using (System.IO.MemoryStream fs = new MemoryStream(dll))
        // {
        //     using (System.IO.MemoryStream p = new MemoryStream(pdb))
        //     {
        //         appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
        //     }
        // }

        InitializeILRuntime();
        OnHotFixLoaded();
    }

    void InitializeILRuntime()
    {
        //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
    }

    void OnHotFixLoaded()
    {
        //HelloWorld，第一次方法调用
        appdomain.Invoke("HotFix_Project.InstanceClass", "StaticFunTest", null, null);

    }

    void Update()
    {

    }
}
