using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyElevator : MonoBehaviour,IInteractable
{
    [SerializeField] ElevatorLocomotion elevator;
    [SerializeField] public EmptyElevator upNeighbor;
    [SerializeField] public EmptyElevator downNeighbor;
    public bool Interact(Interactor interactor)
    {
        Debug.Log("appel elevator");
        if (elevator.currentEmpty != this)
        {
            Debug.Log("call elevator");
        if (!elevator.isMoving) elevator.MoveToEmptyElevator(this);
        }
        return true;
    }



}
