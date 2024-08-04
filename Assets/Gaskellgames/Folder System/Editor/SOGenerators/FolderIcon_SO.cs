#if UNITY_EDITOR
using System.Collections.Generic;
using Gaskellgames.Shared_Resources.Utilities.ProjectUtility;
using UnityEngine;

/// <summary>
/// Code updated by Gaskellgames
/// </summary>

namespace Gaskellgames.FolderSystem
{
    public class FolderIcon_SO : GGScriptableObject
    {
        #region Variables

        public Texture2D icon;
        public List<string> folderNames;

        #endregion
        
    } // class end
}
        
#endif