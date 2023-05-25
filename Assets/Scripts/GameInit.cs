using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace SQDFC
{
    public class GameInit : MonoBehaviour
    {
        [Header("资源运行模式")] public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Debug.Log($"资源系统运行模式：{PlayMode}");

            LoadAsset().Forget();;
            //StartCoroutine(LoadAssets());
        }
        
        async UniTaskVoid LoadAsset()
        {
            // 1.初始化资源系统
            YooAssets.Initialize();

            // 创建默认的资源包
            var package = YooAssets.CreatePackage("DefaultPackage");

            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(package);

            if (PlayMode == EPlayMode.EditorSimulateMode)
            {
                // 编辑器模拟模式
                var initParameters = new EditorSimulateModeParameters();
                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
                await package.InitializeAsync(initParameters);
            }
            else if (PlayMode == EPlayMode.HostPlayMode)
            {
                // 联机运行模式 TODO接CDN服务器
                var initParameters = new HostPlayModeParameters();
                initParameters.QueryServices = new QueryStreamingAssetsFileServices();
                initParameters.DefaultHostServer = "http://192.168.2.26/CDN/v1.0";
                initParameters.FallbackHostServer = "http://192.168.2.26/CDN/v1.0";
                await package.InitializeAsync(initParameters);
            }
            else if (PlayMode == EPlayMode.OfflinePlayMode)
            {
                // 单机模式（测试下可用这种模式，省去挂服务器）
                var initParameters = new OfflinePlayModeParameters();
                await package.InitializeAsync(initParameters);
            }

            //2.获取资源版本
            var operation = package.UpdatePackageVersionAsync();
            await operation;

            if (operation.Status != EOperationStatus.Succeed)
            {
                //更新失败
                Debug.LogError(operation.Error);
                await UniTask.Yield();
                throw new OperationCanceledException();
            }

            string PackageVersion = operation.PackageVersion;

            //3.更新补丁清单
            var operation2 = package.UpdatePackageManifestAsync(PackageVersion);
            await operation2;

            if (operation2.Status != EOperationStatus.Succeed)
            {
                //更新失败
                Debug.LogError(operation2.Error);
                await UniTask.Yield();
                throw new OperationCanceledException();
            }

            //4.下载补丁包
            await Download();
            
            AssetOperationHandle handle2 = package.LoadAssetAsync<GameObject>("Cube");
            await handle2;
            GameObject go = handle2.InstantiateSync();
            Debug.Log($"Prefab name is {go.name}");

            #region 使用示例

            //----------加载场景
            //string location = "scene_home";
            //var sceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single;
            //bool activateOnLoad = true;
            //SceneOperationHandle handle = package.LoadSceneAsync(location, sceneMode, activateOnLoad);
            //yield return handle;
            //Debug.Log($"Scene name is {handle.SceneObject.name}");

            //---------加载prefab
            // AssetOperationHandle handle2 = package.LoadAssetAsync<GameObject>("LoadBattleScene");
            // yield return handle2;
            // GameObject go = handle2.InstantiateSync();
            // Debug.Log($"Prefab name is {go.name}");


            //------------加载原生文件
            //RawFileOperationHandle handle4 = package.LoadRawFileAsync("RawFile");
            //yield return handle4;
            //// byte[] fileData = handle.GetRawFileData();
            //string fileText = handle4.GetRawFileText();
            //Debug.Log("原生文件： " + fileText);

            //--------------访问图集的精灵对象
            //SubAssetsOperationHandle handle5 = package.LoadSubAssetsAsync<Sprite>("sprites");
            //yield return handle5;
            //var sprite = handle5.GetSubAssetObject<Sprite>("sprites_30");
            //Debug.Log($"Sprite name is {sprite.name}");
            //uiHome.transform.Find("Image").GetComponent<Image>().sprite = sprite;


            //----------------委托加载
            //AssetOperationHandle handle6 = package.LoadAssetAsync<GameObject>("player_ship");
            //handle6.Completed += Handle_Completed;

            // void Handle_Completed(AssetOperationHandle handle)
            // {
            //     AudioClip audioClip = handle.AssetObject as AudioClip;
            // }


            //----------------采用task加载
            // async void Start()
            // {
            //     AssetOperationHandle handle = package.LoadAssetAsync<AudioClip>("Assets/GameRes/Audio/bgMusic.mp3");
            //     await handle.Task;
            //     AudioClip audioClip = handle.AssetObject as AudioClip;  
            // }

            //--------通过资源标签来获取资源信息列表。
            //AssetInfo[] assetInfos = package.GetAssetInfos("Effect");
            //foreach (var assetInfo in assetInfos)
            //{
            //    Debug.Log(assetInfo.AssetPath);
            //}

            #endregion
        }


        IEnumerator LoadAssets()
        {
            // 1.初始化资源系统
            YooAssets.Initialize();

            // 创建默认的资源包
            var package = YooAssets.CreatePackage("DefaultPackage");

            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(package);

            if (PlayMode == EPlayMode.EditorSimulateMode)
            {
                // 编辑器模拟模式
                var initParameters = new EditorSimulateModeParameters();
                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
                yield return package.InitializeAsync(initParameters);
            }
            else if (PlayMode == EPlayMode.HostPlayMode)
            {
                // 联机运行模式 TODO接CDN服务器
                var initParameters = new HostPlayModeParameters();
                initParameters.QueryServices = new QueryStreamingAssetsFileServices();
                initParameters.DefaultHostServer = "http://192.168.2.26/CDN/v1.0";
                initParameters.FallbackHostServer = "http://192.168.2.26/CDN/v1.0";
                yield return package.InitializeAsync(initParameters);
            }
            else if (PlayMode == EPlayMode.OfflinePlayMode)
            {
                // 单机模式（测试下可用这种模式，省去挂服务器）
                var initParameters = new OfflinePlayModeParameters();
                yield return package.InitializeAsync(initParameters);
            }

            //2.获取资源版本
            var operation = package.UpdatePackageVersionAsync();
            yield return operation;

            if (operation.Status != EOperationStatus.Succeed)
            {
                //更新失败
                Debug.LogError(operation.Error);
                yield break;
            }

            string PackageVersion = operation.PackageVersion;

            //3.更新补丁清单
            var operation2 = package.UpdatePackageManifestAsync(PackageVersion);
            yield return operation2;

            if (operation2.Status != EOperationStatus.Succeed)
            {
                //更新失败
                Debug.LogError(operation2.Error);
                yield break;
            }

            //4.下载补丁包
            yield return Download();


            // AssetOperationHandle handle2 = package.LoadAssetAsync<GameObject>("Cube");
            // yield return handle2;
            // GameObject go = handle2.InstantiateSync();
            // Debug.Log($"Prefab name is {go.name}");

            #region 使用示例

            //----------加载场景
            //string location = "scene_home";
            //var sceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single;
            //bool activateOnLoad = true;
            //SceneOperationHandle handle = package.LoadSceneAsync(location, sceneMode, activateOnLoad);
            //yield return handle;
            //Debug.Log($"Scene name is {handle.SceneObject.name}");

            //---------加载prefab
            // AssetOperationHandle handle2 = package.LoadAssetAsync<GameObject>("LoadBattleScene");
            // yield return handle2;
            // GameObject go = handle2.InstantiateSync();
            // Debug.Log($"Prefab name is {go.name}");


            //------------加载原生文件
            //RawFileOperationHandle handle4 = package.LoadRawFileAsync("RawFile");
            //yield return handle4;
            //// byte[] fileData = handle.GetRawFileData();
            //string fileText = handle4.GetRawFileText();
            //Debug.Log("原生文件： " + fileText);

            //--------------访问图集的精灵对象
            //SubAssetsOperationHandle handle5 = package.LoadSubAssetsAsync<Sprite>("sprites");
            //yield return handle5;
            //var sprite = handle5.GetSubAssetObject<Sprite>("sprites_30");
            //Debug.Log($"Sprite name is {sprite.name}");
            //uiHome.transform.Find("Image").GetComponent<Image>().sprite = sprite;


            //----------------委托加载
            //AssetOperationHandle handle6 = package.LoadAssetAsync<GameObject>("player_ship");
            //handle6.Completed += Handle_Completed;

            // void Handle_Completed(AssetOperationHandle handle)
            // {
            //     AudioClip audioClip = handle.AssetObject as AudioClip;
            // }


            //----------------采用task加载
            // async void Start()
            // {
            //     AssetOperationHandle handle = package.LoadAssetAsync<AudioClip>("Assets/GameRes/Audio/bgMusic.mp3");
            //     await handle.Task;
            //     AudioClip audioClip = handle.AssetObject as AudioClip;  
            // }

            //--------通过资源标签来获取资源信息列表。
            //AssetInfo[] assetInfos = package.GetAssetInfos("Effect");
            //foreach (var assetInfo in assetInfos)
            //{
            //    Debug.Log(assetInfo.AssetPath);
            //}

            #endregion
        }

        IEnumerator Download()
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            int timeout = 60;
            var package = YooAssets.GetPackage("DefaultPackage");
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain, timeout);

            //没有需要下载的资源
            if (downloader.TotalDownloadCount == 0)
            {
                yield break;
            }

            //需要下载的文件总数和总大小
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;

            //注册回调方法
            downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
            downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
            downloader.OnDownloadOverCallback = OnDownloadOverFunction;
            downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

            //开启下载
            downloader.BeginDownload();
            yield return downloader;

            //检测下载结果
            if (downloader.Status == EOperationStatus.Succeed)
            {
                //下载成功
                Debug.Log("更新完成!");
            }
            else
            {
                //下载失败
                Debug.LogError("更新失败！");
            }
        }

        /// <summary>
        /// 下载出错
        /// </summary>
        private void OnDownloadErrorFunction(string fileName, string error)
        {
            Debug.LogError(string.Format("下载出错：文件名：{0}, 错误信息：{1}", fileName, error));
        }


        /// <summary>
        /// 更新中
        /// </summary>
        private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount,
            long totalDownloadBytes, long currentDownloadBytes)
        {
            Debug.Log(string.Format("文件总数：{0}, 已下载文件数：{1}, 下载总大小：{2}, 已下载大小：{3}", totalDownloadCount,
                currentDownloadCount, totalDownloadBytes, currentDownloadBytes));
        }

        /// <summary>
        /// 下载完成
        /// </summary>
        private void OnDownloadOverFunction(bool isSucceed)
        {
            Debug.Log("下载" + (isSucceed ? "成功" : "失败"));
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        private void OnStartDownloadFileFunction(string fileName, long sizeBytes)
        {
            Debug.Log(string.Format("开始下载：文件名：{0}, 文件大小：{1}", fileName, sizeBytes));
        }
    }
}