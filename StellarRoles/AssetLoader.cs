using Reactor.Utilities.Extensions;
using System.Reflection;
using UnityEngine;

namespace StellarRoles
{
    public static class AssetLoader
    {
        private static readonly Assembly stellarCustomLobby = Assembly.GetExecutingAssembly();

        public static void LoadAssets()
        {

            System.IO.Stream resourceStreamLobby = stellarCustomLobby.GetManifestResourceStream("StellarRoles.Resources.newstellarlobby");
            AssetBundle assetBundleLobby = AssetBundle.LoadFromMemory(resourceStreamLobby.ReadFully());

            StellarRolesPlugin.CustomLobbyPrefab = assetBundleLobby.LoadAsset<GameObject>("allul_customLobby.prefab").DontDestroy();

            assetBundleLobby.Unload(false);

        }
    }
}
