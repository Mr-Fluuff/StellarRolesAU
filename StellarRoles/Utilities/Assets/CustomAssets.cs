using Reactor.Utilities;
using UnityEngine;

namespace StellarRoles.Utilities.Assets
{
    public static class CustomAssets
    {
        public static readonly AssetBundle StellarBundle = AssetBundleManager.Load("stellar-assets");
        public static LoadableAsset<GameObject> CustomLobbyPrefab = new BundleAsset<GameObject>("StellarLobby", StellarBundle);
        public static LoadableAsset<GameObject> Goopy = new BundleAsset<GameObject>("Goopy", StellarBundle);
        public static AssetBundle CosmeticsBundle { get; set; }
        public static LoadableAsset<TextAsset> HatsJsonFile { get; set; }
        public static LoadableAsset<TextAsset> VisorJsonFile { get; set; }
        public static LoadableAsset<TextAsset> NameplateJsonFile { get; set; }
        public static LoadableAsset<TextAsset> PetsJsonFile { get; set; }



        public static void LoadCosmeticsBundle()
        {
            if (CosmeticsBundle == null)
            {
                CosmeticsBundle = AssetBundleManager.Load("stellar-cosmetics");
                HatsJsonFile = new BundleAsset<TextAsset>("CustomHats.json", CosmeticsBundle);
                VisorJsonFile = new BundleAsset<TextAsset>("CustomVisors.json", CosmeticsBundle);
                NameplateJsonFile = new BundleAsset<TextAsset>("CustomPlates.json", CosmeticsBundle);
                PetsJsonFile = new BundleAsset<TextAsset>("CustomPets.json", CosmeticsBundle);

            }
        }
    }
}
