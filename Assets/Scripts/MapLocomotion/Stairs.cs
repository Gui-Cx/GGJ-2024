using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour, IInteractable
{
    [SerializeField] Stairs pairStairs;

    public bool Interact(Interactor interactor)
    {
        interactor.transform.position = pairStairs.transform.position;
        return true;
    }

    private void Start()
    {
        if (pairStairs == null) Debug.LogError(name + " has no stair pair");
        else if (pairStairs == this) Debug.LogWarning(name + "'s stair pair is itself");
    }
}
