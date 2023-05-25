using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using YooAsset.Editor;

public class BuildTools
{
    private static string m_AppName = PlayerSettings.productName;

    // android路径
    public static string m_AndroidPath = Application.dataPath + "/../BuildTarget/Android/";

    public static string m_AndroidBundlePath = Application.dataPath + "/../Bundles/Android/";

    // webgl路径
    public static string m_WebGLPath = Application.dataPath + "/../BuildTarget/WebGL/";
    public static string m_WebGLBundlePath = Application.dataPath + "/../Bundles/WebGL/";
    public static string m_WebGLSeverPath = @"C:\Nginx-1.21.6\html\Healthy_WebGL";
    public static string m_WebGLSeverAddress = "192.168.2.251/Healthy_WebGL/";
    public static string m_WebGLIndexPath = Application.dataPath + "/../Bundles/index.html";

    // ios路径
    public static string m_IOSPath = Application.dataPath + "/../BuildTarget/IOS/";

    // YooAsset 来加载AB 
    private static void BuildInternal(BuildTarget buildTarget)
    {
        Debug.Log($"开始构建 : {buildTarget}");

        // 构建参数
        string defaultOutputRoot = AssetBundleBuilderHelper.GetDefaultOutputRoot();
        BuildParameters buildParameters = new BuildParameters();
        buildParameters.OutputRoot = defaultOutputRoot;
        buildParameters.BuildTarget = buildTarget;
        buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline;
        buildParameters.BuildMode = EBuildMode.ForceRebuild;
        buildParameters.PackageName = "DefaultPackage";
        buildParameters.PackageVersion = "1.0";
        buildParameters.VerifyBuildingResult = true;
        buildParameters.ShareAssetPackRule = new DefaultShareAssetPackRule();
        buildParameters.CompressOption = ECompressOption.LZ4;
        buildParameters.OutputNameStyle = EOutputNameStyle.HashName;
        buildParameters.CopyBuildinFileOption = ECopyBuildinFileOption.ClearAndCopyAll;

        // 执行构建
        AssetBundleBuilder builder = new AssetBundleBuilder();
        var buildResult = builder.Run(buildParameters);
        if (buildResult.Success)
        {
            Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
        }
        else
        {
            Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
        }
    }

    // 本地打包测试
    [MenuItem("Tools/Build")]
    public static void Build()
    {
        //生成可执行程序
        // string abPath = Application.dataPath + "/../AssetBundle/" +
        //                 EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
        // Copy(abPath, Application.streamingAssetsPath);
        string savePath = "";
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            PlayerSettings.Android.keystorePass = "packer";
            PlayerSettings.Android.keyaliasPass = "packer";
            PlayerSettings.Android.keyaliasName = "android.keystore";
            PlayerSettings.Android.keystoreName = Application.dataPath.Replace("/Assets", "") + "/healthy.keystore";
            savePath = m_AndroidPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget +
                       string.Format("_{0:MM_dd_HH_mm}", DateTime.Now) + ".apk";
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            savePath = m_IOSPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget +
                       string.Format("_{0:MM_dd_HH_mm}", DateTime.Now);
        }

        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), savePath, EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None);
        // DeleteDir(Application.streamingAssetsPath);
    }

    #region 打包Android

    [MenuItem("Tools/BuildAPK")]
    public static void BuildAndroid()
    {
        Debug.Log("Enter Build Android...");

        // 清除上次遗留的AB包
        DeleteDir(m_AndroidBundlePath);

        // 清除上次遗留的主体包
        DeleteDir(m_AndroidPath);

        // 打ab包
        BuildInternal(BuildTarget.Android);

        // 打主体包
        PlayerSettings.Android.keystorePass = "packer";
        PlayerSettings.Android.keyaliasPass = "packer";
        PlayerSettings.Android.keyaliasName = "android.keystore";
        PlayerSettings.Android.keystoreName = Application.dataPath.Replace("/Assets", "") + "/healthy.keystore";
        BuildSetting buildSetting = GetAndroidBuildSetting();

        string suffix = SetAndroidSetting(buildSetting);
        string savePath = m_AndroidPath + m_AppName + "_Andorid" + suffix +
                          string.Format("_{0:MM_dd_HH_mm}.apk", DateTime.Now);
        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), savePath, EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None);
    }

    static BuildSetting GetAndroidBuildSetting()
    {
        string[] parameters = Environment.GetCommandLineArgs();
        BuildSetting buildSetting = new BuildSetting();
        foreach (string str in parameters)
        {
            if (str.StartsWith("Place"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Place = (Place)Enum.Parse(typeof(Place), tempParam[1], true);
                }
            }
            else if (str.StartsWith("Version"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Version = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Build"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Build = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Name"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Name = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Debug"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.Debug);
                }
            }
            else if (str.StartsWith("MulRendering"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.MulRendering);
                }
            }
            else if (str.StartsWith("IL2CPP"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.IL2CPP);
                }
            }
        }

        return buildSetting;
    }

    static string SetAndroidSetting(BuildSetting setting)
    {
        string suffix = "_";
        if (setting.Place != Place.None)
        {
            //代表了渠道包
            string symbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,
                symbol + ";" + setting.Place.ToString());
            suffix += setting.Place.ToString();
        }

        if (!string.IsNullOrEmpty(setting.Version))
        {
            PlayerSettings.bundleVersion = setting.Version;
            suffix += setting.Version;
        }

        if (!string.IsNullOrEmpty(setting.Build))
        {
            PlayerSettings.Android.bundleVersionCode = int.Parse(setting.Build);
            suffix += "_" + setting.Build;
        }

        if (!string.IsNullOrEmpty(setting.Name))
        {
            PlayerSettings.productName = setting.Name;
            //PlayerSettings.applicationIdentifier = "com.TTT." + setting.Name;
        }

        if (setting.MulRendering)
        {
            PlayerSettings.MTRendering = true;
            suffix += "_MTR";
        }
        else
        {
            PlayerSettings.MTRendering = false;
        }

        if (setting.IL2CPP)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            suffix += "_IL2CPP";
        }
        else
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        }

        if (setting.Debug)
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = true;
            suffix += "_Debug";
        }
        else
        {
            EditorUserBuildSettings.development = false;
        }

        return suffix;
    }

    #endregion


    #region 打包WebGL

    public static void BuildWebGL()
    {
        Debug.Log("Enter Build WebGL...");

        // 清除上次遗留的AB包
        DeleteDir(m_WebGLBundlePath);

        // 清除上次遗留的主体包
        DeleteDir(m_WebGLPath);

        // 打ab包
        BuildInternal(BuildTarget.WebGL);

        // 打主体包
        BuildSetting buildSetting = GetWebGLBuildSetting();

        string suffix = SetWebGLSetting(buildSetting);
        string savePath = m_WebGLPath + m_AppName + "_WebGL" + suffix +
                          string.Format("_{0:MM_dd_HH_mm}", DateTime.Now);
        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), savePath, EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None);

        // 复制到本地的服务器上
        Copy(m_WebGLPath, m_WebGLSeverPath);

        // 输出服务器下载地址
        WriteBuildName(GetDownLoadAddress(savePath));
    }

    static BuildSetting GetWebGLBuildSetting()
    {
        string[] parameters = Environment.GetCommandLineArgs();
        BuildSetting buildSetting = new BuildSetting();
        foreach (string str in parameters)
        {
            if (str.StartsWith("Place"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Place = (Place)Enum.Parse(typeof(Place), tempParam[1], true);
                }
            }
            else if (str.StartsWith("Version"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Version = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Build"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Build = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Name"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Name = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Debug"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.Debug);
                }
            }
            else if (str.StartsWith("MulRendering"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.MulRendering);
                }
            }
            else if (str.StartsWith("IL2CPP"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.IL2CPP);
                }
            }
        }

        return buildSetting;
    }

    static string SetWebGLSetting(BuildSetting setting)
    {
        string suffix = "_";
        if (setting.Place != Place.None)
        {
            //代表了渠道包
            string symbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,
                symbol + ";" + setting.Place.ToString());
            suffix += setting.Place.ToString();
        }

        if (!string.IsNullOrEmpty(setting.Version))
        {
            PlayerSettings.bundleVersion = setting.Version;
            suffix += setting.Version;
        }

        if (!string.IsNullOrEmpty(setting.Build))
        {
            PlayerSettings.Android.bundleVersionCode = int.Parse(setting.Build);
            suffix += "_" + setting.Build;
        }

        if (!string.IsNullOrEmpty(setting.Name))
        {
            PlayerSettings.productName = setting.Name;
            //PlayerSettings.applicationIdentifier = "com.TTT." + setting.Name;
        }

        if (setting.MulRendering)
        {
            PlayerSettings.MTRendering = true;
            suffix += "_MTR";
        }
        else
        {
            PlayerSettings.MTRendering = false;
        }

        if (setting.IL2CPP)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            suffix += "_IL2CPP";
        }
        else
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        }

        if (setting.Debug)
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = true;
            suffix += "_Debug";
        }
        else
        {
            EditorUserBuildSettings.development = false;
        }

        return suffix;
    }

    #endregion


    #region 打包IOS

    public static void BuildIOS()
    {
        Debug.Log("Enter Build iOS...");

        // 清除上次遗留的打包文件
        DeleteDir(m_IOSPath);

        // 打ab包
        BuildInternal(BuildTarget.iOS);

        // 打主体包
        BuildSetting buildSetting = GetIOSBuildSetting();
        string suffix = SetIOSSetting(buildSetting);
        string name = m_AppName + "_IOS" + suffix + string.Format("_{0:MM_dd_HH_mm}", DateTime.Now);
        string savePath = m_IOSPath + name;
        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), savePath, EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None);

        //WriteBuildName(name);
    }

    static BuildSetting GetIOSBuildSetting()
    {
        string[] parameters = Environment.GetCommandLineArgs();
        BuildSetting buildSetting = new BuildSetting();
        foreach (string str in parameters)
        {
            if (str.StartsWith("Version"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Version = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Build"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Build = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Name"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Name = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("MulRendering"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.MulRendering);
                }
            }
            else if (str.StartsWith("DynamicBatching"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.DynamicBatching);
                }
            }
        }

        return buildSetting;
    }

    static string SetIOSSetting(BuildSetting setting)
    {
        string suffix = "_";

        if (!string.IsNullOrEmpty(setting.Version))
        {
            PlayerSettings.bundleVersion = setting.Version;
            suffix += setting.Version;
        }

        if (!string.IsNullOrEmpty(setting.Build))
        {
            PlayerSettings.iOS.buildNumber = setting.Build;
            suffix += "_" + setting.Build;
        }

        if (!string.IsNullOrEmpty(setting.Name))
        {
            PlayerSettings.productName = setting.Name;
            //PlayerSettings.applicationIdentifier = "com.TTT." + setting.Name;
        }

        if (setting.MulRendering)
        {
            PlayerSettings.MTRendering = true;
            suffix += "_MTR";
        }
        else
        {
            PlayerSettings.MTRendering = false;
        }

        if (setting.DynamicBatching)
        {
            suffix += "_Dynamic";
        }
        else
        {
        }

        return suffix;
    }

    #endregion

    private static string[] FindEnableEditorrScenes()
    {
        List<string> editorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            editorScenes.Add(scene.path);
        }

        return editorScenes.ToArray();
    }

    private static void Copy(string srcPath, string targetPath)
    {
        try
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            string scrdir = Path.Combine(targetPath, Path.GetFileName(srcPath));
            if (Directory.Exists(srcPath))
                scrdir += Path.DirectorySeparatorChar;
            if (!Directory.Exists(scrdir))
            {
                Directory.CreateDirectory(scrdir);
            }

            string[] files = Directory.GetFileSystemEntries(srcPath);
            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    Copy(file, scrdir);
                }
                else
                {
                    File.Copy(file, scrdir + Path.GetFileName(file), true);
                }
            }
        }
        catch
        {
            Debug.LogError("无法复制：" + srcPath + "  到" + targetPath);
        }
    }

    public static void DeleteDir(string scrPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(scrPath);
            FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo info in fileInfo)
            {
                if (info is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(info.FullName);
                    subdir.Delete(true);
                }
                else
                {
                    File.Delete(info.FullName);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    // 将下载地址写入文本中
    public static void WriteBuildName(string name)
    {
        FileInfo fileInfo = new FileInfo(m_WebGLPath + "buildname.txt");
        StreamWriter sw = fileInfo.CreateText();
        sw.WriteLine(name);
        sw.Close();
        sw.Dispose();
    }


    public static string GetDownLoadAddress(string address)
    {
        string[] splits = address.Split("/");
        return m_WebGLSeverAddress + splits[splits.Length - 1];
    }
}

public class BuildSetting
{
    //版本号
    public string Version = "";

    //build次数
    public string Build = "";

    //程序名称
    public string Name = "";

    //是否debug
    public bool Debug = true;

    //渠道
    public Place Place = Place.None;

    //多线程渲染
    public bool MulRendering = true;

    //是否IL2CPP
    public bool IL2CPP = false;

    //是否开启动态合批
    public bool DynamicBatching = false;
}

public enum Place
{
    None = 0,
    Weixin,
}