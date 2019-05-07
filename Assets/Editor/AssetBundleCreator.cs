using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Class for AssetBundle creation functions
/// </summary>
public class AssetBundleCreator
{
    /// <summary>
    /// Function for creating an AssetBundle.
    /// Using [MenuItem("")] it creates an option under the "Assets" tab 
    /// in the editor where this function will be called from.
    /// Must be called every time a change happens in the VulnerabilityLogic.xml file.
    /// </summary>
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = Path.Combine(Application.streamingAssetsPath, "Assets/AssetBundles");
        if (!Directory.Exists(assetBundleDirectory))
            Directory.CreateDirectory(assetBundleDirectory);

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }


}
