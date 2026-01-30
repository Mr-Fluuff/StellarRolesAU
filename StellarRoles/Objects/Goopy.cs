using System.Linq;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class Goopy
    {
        private static GameObject GoopyGameObject;
        private static Console TempHot;

        public static void CreateGoopy()
        {
            if (GoopyGameObject == null)
                GoopyGameObject = UnityEngine.Object.Instantiate(CustomAssets.Goopy.LoadAsset());

            Vector3 position = new(50f, 50f);
            TempHot = UnityEngine.Object.FindObjectsOfType<Console>().ToList().Find(console => console.name == "panel_temphot");
            if (TempHot != null)
            {
                GoopyGameObject.transform.SetParent(TempHot.transform);
                position = TempHot.gameObject.transform.position;
            }
            position.x += 2.4f;
            position.y -= 1f;
            GoopyGameObject.transform.position = position;
            var script = GoopyGameObject.AddComponent<ProximityBehaviour>();
            script.animator = GoopyGameObject.GetComponent<Animator>();
            GoopyGameObject.SetActive(true);
        }

        public static void ClearGoopy()
        {
            GoopyGameObject = null;
        }
    }
}