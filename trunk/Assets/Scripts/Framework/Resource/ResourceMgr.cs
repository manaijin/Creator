using Framework.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Debug = Framework.Util.Debug;

namespace Framework
{

    /**
     需求：
    ×1.不同格式的资源加载：Texture\Sprite\
    √2.资源查询
    √3.多个资源加载
    √4.重写实例化方法
    √5.资源卸载
         */

    /// <summary>
    /// 资源管理类
    /// 1.Asset加载、卸载
    /// 2.GameObject实例化
    /// </summary>
    public class ResourceMgr : Singleton<ResourceMgr>
    {
        private static readonly Dictionary<string, IResourceLocation> checkPool;
        private static readonly Dictionary<string, AsyncOperationHandle> assetPool;

        static ResourceMgr()
        {            
            checkPool = new Dictionary<string, IResourceLocation>();
            assetPool = new Dictionary<string, AsyncOperationHandle>();
        }

        public GameObject Instantiate(string key)
        {
            return Instantiate<GameObject>(key);
        }

        public T Instantiate<T>(string key) where T : UnityEngine.Object
        {
            T result = null;
            if (assetPool.ContainsKey(key))
            {
                var asset = assetPool[key].Result as T;
                if (asset != null)
                {
                    result = GameObject.Instantiate<T>(asset);
                }
                else
                {
                    Debug.LogError("Asset {0} can not convert to {1}", key, typeof(T));
                }
            }
            else
            {
                Debug.LogError("Asset: {0} is not Loaded", key);
            }
            return result;
        }

        public AsyncResult InstantiateAsync(IList<string> key, Action<GameObject> callBack = null) 
        {
            return InstantiateAsync(key, callBack);
        }

        public AsyncResult InstantiateAsync<T>(IList<string> key, Action<T> callBack = null) where T : class
        {
            var result = new AsyncResult();
            Action<GameObject> fun = (GameObject obj) =>
            {
                result.IsDone = true;
                result.Result = obj;
                callBack?.Invoke(obj as T);
            };

            if (checkPool.ContainsKey(key[0]))
            {
                InstantiateIRes(checkPool[key[0]], fun);
            }
            else
            {
                LoadAssetAsync(key[0], (obj) =>
                {
                    if (obj == null)
                        return;

                    var h = Addressables.InstantiateAsync(key[0]);
                    h.Completed += (hand) =>
                    {
                        fun(hand.Result);
                    };
                });
            }
            return result;
        }

        public AsyncResult InstantiateAsync(string key, Action<GameObject> callBack = null) 
        {
            return InstantiateAsync<GameObject>(key, callBack);
        }

        public AsyncResult InstantiateAsync<T>(string key, Action<T> callBack = null) where T: class
        {
            var result = new AsyncResult();
            Action<GameObject> fun = (GameObject obj) =>
            {
                result.IsDone = true;
                result.Result = obj;
                callBack?.Invoke(obj as T);
            };

            if (checkPool.ContainsKey(key))
            {
                InstantiateIRes(checkPool[key], fun);
            }
            else
            {
                LoadAssetAsync(key, (obj) =>
                {
                    if (obj == null)
                        return;

                    var h = Addressables.InstantiateAsync(key);
                    h.Completed += (hand) =>
                    {
                        fun(hand.Result);
                    };
                });
            }
            return result;
        }

        public AsyncResult LoadAssetsAsync(IList<string> keys, Action endCallBack = null, Action<UnityEngine.Object> eachCallBack = null)
        {
            var result = new AsyncResult();
            int m_ListCount = keys.Count;
            foreach (var key in keys)
            {
                LoadAssetAsync(key, (obj) =>
                {
                    eachCallBack?.Invoke(obj);
                    m_ListCount--;
                    if (m_ListCount <= 0)
                    {
                        result.IsDone = true;
                        endCallBack?.Invoke();
                    }
                });
            }
            return result;
        }

        public AsyncResult LoadAssetAsync(string key, Action<UnityEngine.Object> callBack = null)
        {
            var result = new AsyncResult();
            if (!checkPool.ContainsKey(key))
            {
                CheckAssetExist(key, (ires) =>
                {
                    AsyncOperationHandle<UnityEngine.Object> h = Addressables.LoadAssetAsync<UnityEngine.Object>(ires);
                    h.Completed += (obj) =>
                    {
                        result.IsDone = true;
                        result.Result = obj.Result;
                        callBack?.Invoke(obj.Result);
                        if (obj.Result != null)
                        {
                            if(!assetPool.ContainsKey(key))
                                assetPool.Add(key, h);
                        }
                        else
                        {
                            Addressables.Release(h);
                            Debug.Log("The asset {0} is not exist", key);
                        }
                    };
                });
            }
            else
            {
                Debug.Log("The asset {0} is exist", key);
                result.IsDone = true;
                result.Result = checkPool[key];
            }
            return result;
        }

        [Obsolete]
        public AsyncResult LoadAssetAsync(IList<string> key, Action<UnityEngine.Object> callBack = null)
        {
            var result = new AsyncResult();
            if (!checkPool.ContainsKey(key[0]))
            {
                CheckAssetExist(key, (ires) =>
                {
                    AsyncOperationHandle<UnityEngine.Object> h = Addressables.LoadAssetAsync<UnityEngine.Object>(ires);
                    h.Completed += (obj) =>
                    {
                        result.IsDone = true;
                        result.Result = obj.Result;
                        assetPool.Add(key[0], h);
                        callBack?.Invoke(obj.Result);
                    };
                });
            }
            else
            {
                Debug.LogError("The asset {0} is exist", key[0]);
                result.IsDone = true;
                result.Result = checkPool[key[0]];
            }
            return result;
        }

        private void InstantiateIRes(IResourceLocation ires, Action<GameObject> callBack = null)
        {
            if (ires == null)
                return;

            var handler = Addressables.InstantiateAsync(ires);
            handler.Completed += (obj) =>
            {
                callBack?.Invoke(obj.Result);
            };
        }

        private AsyncOperationHandle<IList<IResourceLocation>> CheckAssetExist(string key, Action<IResourceLocation> callBack = null)
        {
            var assetHandle = Addressables.LoadResourceLocationsAsync(key);
            assetHandle.Completed += (inquiryData) =>
            {
                if (inquiryData.Result.Count == 0)
                {
                    Debug.LogError("key:{0} is not exist", key);
                    return;
                }

                if (inquiryData.Result.Count > 1)
                {
                    Debug.LogError("key:{0} - asset is more then one", key);
                    return;
                }

                if(!checkPool.ContainsKey(key))
                    checkPool.Add(key, assetHandle.Result[0]);
                callBack?.Invoke(inquiryData.Result[0]);
            };
            return assetHandle;
        }

        [Obsolete]
        public void CheckAssetExist(IList<string> keys, Action<IResourceLocation> callBack)
        {
            var handler = Addressables.LoadResourceLocationsAsync(keys as IList<object>, Addressables.MergeMode.Intersection);
            
            handler.Completed += (inquiryData) =>
            {
                if (inquiryData.Result.Count == 0)
                {
                    Debug.LogError("keys is not exist:{0}", ArrayUtil.ToString(keys));
                    callBack?.Invoke(null);
                    return;
                }

                if (inquiryData.Result.Count > 1)
                {
                    Debug.LogError("key asset is more then one:{0}", ArrayUtil.ToString(keys));
                }

                checkPool.Add(keys[0], handler.Result[0]);
                callBack?.Invoke(inquiryData.Result[0]);
            };
        }

        public void UnAsset(string key)
        {
            if (assetPool.ContainsKey(key))
            {
                var asset = assetPool[key];
                var check = checkPool[key];
                Addressables.Release(asset);
                assetPool.Remove(key);
            }
            else
            {
                Debug.LogError("asset {0} is not exist", key);
            }
        }
    }

    public class AsyncResult
    {
        public bool IsDone { get; set; }

        public object Result { get; set; }

        public AsyncResult()
        {
            IsDone = false;
            Result = null;
        }
    }
}