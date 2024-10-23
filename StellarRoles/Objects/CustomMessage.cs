using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles.Objects
{

    public class CustomMessage
    {

        private TMPro.TMP_Text text;
        private static List<CustomMessage> customMessages = new();

        public CustomMessage(string message, float duration, bool flash, Color color)
        {
            RoomTracker roomTracker = HudManager.Instance?.roomTracker;
            if (roomTracker != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);

                gameObject.transform.SetParent(HudManager.Instance.transform);
                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                text = gameObject.GetComponent<TMPro.TMP_Text>();
                text.text = message;

                // Use local position to place it in the player's view instead of the world location
                gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                customMessages.Add(this);

                HudManager.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
                {
                    bool even = ((int)(p * duration / 0.25f)) % 2 == 0; // Bool flips every 0.25 seconds
                    /*                    string prefix = (even ? "<color=#FCBA03FF>" : "<color=#FF0000FF>");
                                        text.text = prefix + message + "</color>";
                    */
                    if (text != null) text.color = flash ? even ? Color.yellow : color : color;
                    if (p == 1f && text != null && text.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(text.gameObject);
                        customMessages.Remove(this);
                    }
                })));
            }
        }
    }
}