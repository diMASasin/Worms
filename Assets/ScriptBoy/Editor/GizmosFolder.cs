using UnityEngine;
using UnityEditor;
using System.IO;

namespace ScriptBoy
{
    // The Gizmos folder must be placed in the root of the Project.
    // This class moves Gizmos from "Assets/ScriptBoy/Gizmos" to "Assets/Gizmos".

    public class GizmosFolder
    {
        [InitializeOnLoadMethod]
        public static void Fix()
        {
            string assets = Application.dataPath;
            string assets_Gizmos = Application.dataPath + "/Gizmos";
            string assets_ScriptBoy_Gizmos = assets + "/ScriptBoy/Gizmos";

            if (Directory.Exists(assets_ScriptBoy_Gizmos))
            {
                if (!Directory.Exists(assets_Gizmos))
                {
                    Directory.CreateDirectory(assets_Gizmos);
                }
                
                string[] files = Directory.GetFiles(assets_ScriptBoy_Gizmos, "*.png*", SearchOption.AllDirectories);
                foreach (var path in files)
                {
                    if (path.EndsWith(".meta")) continue;

                    string _Path = path.Remove(0, assets_ScriptBoy_Gizmos.Length);
                    string assets_Gizmos_Path = assets_Gizmos + _Path;

                    if (File.Exists(assets_Gizmos_Path))
                    {
                        File.Delete(assets_Gizmos_Path);
                        File.Move(path, assets_Gizmos_Path);
                    }
                    else
                    {
                        string[] array = _Path.Split('/','\\');
                        assets_Gizmos_Path = assets_Gizmos;
                        for (int i = 1; i < array.Length - 1; i++)
                        {
                            assets_Gizmos_Path += "/" + array[i];
                            if (!Directory.Exists(assets_Gizmos_Path))
                            {
                                Directory.CreateDirectory(assets_Gizmos_Path);
                            }
                        }
                        assets_Gizmos_Path += "/" + array[array.Length - 1];

                        File.Move(path, assets_Gizmos_Path);
                    }
                }

                FileUtil.DeleteFileOrDirectory(assets_ScriptBoy_Gizmos);
                FileUtil.DeleteFileOrDirectory(assets_ScriptBoy_Gizmos + ".meta");
                AssetDatabase.Refresh();
            }
        }
    }
}