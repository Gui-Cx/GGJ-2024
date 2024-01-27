using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyElevator : MonoBehaviour,IInteractable
{
    [SerializeField] ElevatorLocomotion elevator;
    [SerializeField] public EmptyElevator upNeighbor;
    [SerializeField] public EmptyElevator downNeighbor;


    public BoxCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public bool Interact(Interactor interactor)
    {
        if (elevator.currentEmpty != this && !elevator.isMoving)
        {
            Debug.Log("call elevator");
            elevator.MoveToEmptyElevator(this);
        }
        return true;
    }



}
