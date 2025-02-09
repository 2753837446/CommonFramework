﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace EasyAssets
{
    public partial class EASY_DEFINE
    {
        public const string ASSET_LIST_NAME = "assetlist";
        public const string BUNDLE_INFO_NAME = "bundleinfo";
        public const string BUILDIN_SCENES_NAME = "buildinscenes";

        public const string ASSET_LIST_FILE = "assetlist.bytes";
        public const string BUNDLE_INFO_FILE = "bundleinfo.bytes";
        public const string BUILDIN_SCENES_FILE = "buildinscenes.bytes";
    }

    public static class Setting
    {
        public static EasyAssetConfig config { get; private set; }
        public static string LoadPath { get; private set; }
        public static float DisposeCacheTime { get; private set; } = 5f;
        public static float RefrenceCheckTime { get; private set; } = 1f;
        public static float AssetBundleLiveTime { get; private set; } = 5f;

        public static string RemoteRootDomain { get; private set; }
        public static float RequestTimeOut { get; private set; }

        public static List<string> UnmanagedBundles = new List<string>();

        public enum BundleCheckMode
        {
            /// <summary>
            /// 使用md5值校验 
            /// </summary>
            MD5,
            /// <summary>
            /// 使用文件大小校验
            /// </summary>
            FILE_LENGTH,
        }
        public static BundleCheckMode bundleCheckMode { get; private set; } = BundleCheckMode.MD5;

        static bool inited = false;
        public static void InitSetting()
        {
            if (inited)
                return;
            config = Resources.Load<EasyAssetConfig>("EasyAssetConfig");
            LoadPath = config.LoadPath;
            DisposeCacheTime = config.DisposeCacheTime;
            RefrenceCheckTime = config.RefrenceCheckTime;
            AssetBundleLiveTime = config.AssetBundleLiveTime;

            RemoteRootDomain = config.RemoteBundleRootDomain;
            RequestTimeOut = config.RequestTimeOut;
            bundleCheckMode = config.bundleCheckMode;
            UnmanagedBundles = config.UnmanagedBundles;
            inited = true;
        }
    }

    //路径管理
    public class PathHelper
    {
        static bool inited = false;
        public static string EXTERNAL_ASSET_PATH { get; private set; }

        static string AutoFillPathRoot;
        static Dictionary<string, AssetExtentions> AssetExtentionsMap = new Dictionary<string, AssetExtentions>();

        public static void Init(string path)
        {
            if (inited)
                return;
            inited = true;
            EXTERNAL_ASSET_PATH = Application.persistentDataPath + path + "/";
            if (!Directory.Exists(EXTERNAL_ASSET_PATH))
                Directory.CreateDirectory(EXTERNAL_ASSET_PATH);

            AutoFillPathRoot = "assets/" + Setting.config.AutoFillPathRoot
                + (string.IsNullOrEmpty(Setting.config.AutoFillPathRoot) ? "" : "/");
            var aem_config = Setting.config.AssetExtentionsMap;
            foreach (var ae in aem_config)
                AssetExtentionsMap.Add("UnityEngine." + ae.Type, ae);
        }

        //自动填充外部资源路径
        public static string[] AutoFillExAssetPath<T>(string asset_path)
            where T : UnityEngine.Object
        {
            string ex = Path.GetExtension(asset_path);
            if (!string.IsNullOrEmpty(ex))
                return new string[] { asset_path };     //路径中含有扩展名,返回原路径。

            //如果路径中不包含扩展名，根据资源类型填充扩展名
            string atype = typeof(T).ToString();
            AssetExtentions ae = null;
            if (AssetExtentionsMap.TryGetValue(atype, out ae))
            {
                string[] ps = new string[ae.Extentions.Count];
                for (int i = 0, l = ps.Length; i < l; i++)
                    ps[i] = (AutoFillPathRoot + asset_path + ae.Extentions[i]).ToLower();  //拼接扩展名
                    
                return ps;
            }

            return new string[] { (asset_path + ".asset").ToLower() };
        }
    }

    public class SceneHelper
    {
        static bool inited = false;

        static Dictionary<string, bool> buildInSceneMap = new Dictionary<string, bool>();

        public static void Init()
        {
            if (inited)
                return;

            StreamReader sr = null;
            var path = PathHelper.EXTERNAL_ASSET_PATH + EASY_DEFINE.BUILDIN_SCENES_FILE;

            if (!File.Exists(path))
            {
                var ta = Resources.Load<TextAsset>(EASY_DEFINE.BUILDIN_SCENES_NAME);
                if (ta == null)
                    return;
                using (MemoryStream ms = new MemoryStream(ta.bytes))
                {
                    sr = new StreamReader(ms);
                    LoadFromStream(sr);
                    inited = true;
                    return;
                }
            }

            sr = File.OpenText(path);
            if (sr == null)
                return;

            LoadFromStream(sr);
            inited = true;
        }

        static void LoadFromStream(StreamReader sr)
        {
            using (sr)
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var subs = line.Split(':');
                    var sceneName = ScenePath2SceneName(subs[0]);
                    bool enable = bool.Parse(subs[1]);

                    if (!buildInSceneMap.ContainsKey(sceneName))
                        buildInSceneMap.Add(sceneName, enable);
                    //Debug.Log("Build in Scene : " + sceneName + "," + enable);
                }
            }
        }

        public static string ScenePath2SceneName(string scenePath)
        {
            string name = "";
            try
            {
                name = Path.GetFileName(scenePath).Replace(Path.GetExtension(scenePath), "");
            }
            catch (Exception ex)
            {

            }
            return name;
        }

        public static bool isBuildInSceneByPath(string scenePath)
        {
            var sceneName = ScenePath2SceneName(scenePath);
            return isBuildInSceneByName(sceneName);
        }

        public static bool isBuildInSceneByName(string sceneName)
        {
            if (!inited)
                return true;

            if (buildInSceneMap.ContainsKey(sceneName))
                return buildInSceneMap[sceneName];
            return false;
        }
    }

}