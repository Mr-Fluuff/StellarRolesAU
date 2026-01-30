using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

[RegisterInIl2Cpp]
public class ProximityBehaviour : MonoBehaviour
{
    public Animator animator;

    public ProximityBehaviour(IntPtr ptr) : base(ptr) { } // Required for IL2CPP

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerControl>() != null)
        {
            animator.SetTrigger("Activate");
        }
    }
}
