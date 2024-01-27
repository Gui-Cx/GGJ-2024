using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEditor.Progress;
using JetBrains.Annotations;

enum PlayerState
{
    Idle,
    InElevator
}


public class Player : MonoBehaviour
{
    public int speed = 0;
    // public int maxVelocity = 0;
    // Left = -1; None = 0; Right = 1
    private int movementDirection = 0;
    private int verticalMovementDirection = 0;

    private Rigidbody2D rigidbody2d;
    private Interactor interactor;
    // Start is called before the first frame update
    private ItemController itemController;
    PlayerState currentState;
    ElevatorLocomotion currentElevator;
    Animator animator;

    ITEM_TYPE currentItem;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        interactor = GetComponent<Interactor>();
        currentState = PlayerState.Idle;
        animator = GetComponent<Animator>();
        itemController = GetComponent<ItemController>();
    }    

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                rigidbody2d.AddForce(speed*Vector2.right * movementDirection);
                if (movementDirection != 0)
                {
                    animator.SetBool("isMoving", true);
                    transform.localScale =new Vector3(movementDirection, 1,0);
                }
                else animator.SetBool("isMoving", false);
                break;
            case PlayerState.InElevator:
                transform.position = currentElevator.transform.position;
                if (movementDirection != 0) { TryQuitElevator(movementDirection); }
                if (verticalMovementDirection != 0) { TryUseElevator(verticalMovementDirection); }
                break;
        }
    }
    
    void OnUseItem(InputValue context)
    {
        SetCurrentItem(ITEM_TYPE.Hug);
        Debug.LogFormat("Cx : UseItem");
        
    }

    void OnInteract(InputValue context)
    {
        Debug.LogFormat("Cx : Interact");
        if (interactor.currentInteractable != null) interactor.currentInteractable.Interact(interactor);
        
    }

    void OnMove(InputValue context)
    {
        Debug.LogFormat("Cx : Direction is {0}", context.Get<float>());
        movementDirection = (int)Math.Round(context.Get<float>());
    }

    void OnElevator(InputValue context)
    {
        Debug.LogFormat("Cx : Direction is {0}", context.Get<float>());
        verticalMovementDirection = (int)Math.Round(context.Get<float>());

    }

    public void EnterInElevator(ElevatorLocomotion elevator)
    {
        currentState = PlayerState.InElevator;
        currentElevator = elevator;
        GetComponent<BoxCollider2D>().enabled = false;
        rigidbody2d.simulated = false;
        transform.position = elevator.transform.position;
    }

    public void TryUseElevator(float movementDirection)
    {
        if (!currentElevator.isMoving)
        {
            if (movementDirection < 0) currentElevator.UseElevator(false);
            else currentElevator.UseElevator(true);
        }
            
    }
    public void TryQuitElevator(float movementDirection)
    {
        if (!currentElevator.isMoving)
        {
            currentState = PlayerState.Idle;
            GetComponent<BoxCollider2D>().enabled = true;
            if (movementDirection < 0) transform.position +=  new Vector3(-1f,0,0);
            else transform.position += new Vector3(1f, 0, 0);
            rigidbody2d.simulated = true;
            currentElevator.QuitElevator();
        }
    }


    void SetCurrentItem(ITEM_TYPE item)
    {
        currentItem = item;
        switch (currentItem)
        {
            case ITEM_TYPE.Hug:
                animator.SetTrigger("GetPlush");
                break;
            default:
                animator.SetTrigger("GetEmpty");
                break;
        }
    }
}
