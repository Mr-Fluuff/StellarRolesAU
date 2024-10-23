using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class CustomPassiveButton
    {
        public PassiveButton button;
        public CustomPassiveButton(Transform transform)
        {
            var buttonref = HudManager.Instance.MapButton.GetComponent<PassiveButton>();
            var b = UnityEngine.Object.Instantiate(buttonref);
            b.GetComponent<AspectPosition>().Destroy();
            b.gameObject.SetActive(true);
            b.transform.SetParent(transform);
            b.transform.localPosition = Vector3.zero;
            b.transform.DestroyChildren();
            b.OnClick.RemoveAllListeners();
            b.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            b.OnMouseOut.RemoveAllListeners();
            b.OnMouseOut = new UnityEngine.Events.UnityEvent();
            b.OnMouseOver.RemoveAllListeners();
            b.OnMouseOver = new UnityEngine.Events.UnityEvent();

            button = b;
        }
    }
}
