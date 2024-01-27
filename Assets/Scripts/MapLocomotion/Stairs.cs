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
        if (pairStairs == null) Debug.LogError("No pairStairs");
        else if (pairStairs.pairStairs!=this) Debug.LogError("pairStairs have another Pair");
    }
}
