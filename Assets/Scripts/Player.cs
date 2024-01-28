using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Random = System.Random;

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

    [SerializeField] ITEM_TYPE currentItem;

    public bool IsBoostedByHappinessAOE=false;

    public bool isFacingRight=true;

    float timingHoldUseItem;
    bool isPressedThrow;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        interactor = GetComponent<Interactor>();
        currentState = PlayerState.Idle;
        animator = GetComponent<Animator>();
        itemController = GetComponent<ItemController>();
    }    

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                rigidbody2d.AddForce(speed*Vector2.right * movementDirection);
                if (movementDirection != 0)
                {
                    animator.SetBool("isMoving", true);
                    if (movementDirection > 0 && !isFacingRight) Flip();
                    if (movementDirection < 0 && isFacingRight) Flip();
                }
                else animator.SetBool("isMoving", false);
                break;
            case PlayerState.InElevator:
                transform.position = currentElevator.transform.position;
                if (movementDirection != 0) { TryQuitElevator(movementDirection); }
                if (verticalMovementDirection != 0) { TryUseElevator(verticalMovementDirection); }
                break;
        }
        if(isPressedThrow) itemController.GetTimeHold(Time.time - timingHoldUseItem);

    }

    void Flip()
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = !gameObject.GetComponent<SpriteRenderer>().flipX;
        isFacingRight=!isFacingRight;
    }
    
    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isPressedThrow = true;
            itemController.OnItemUsed(currentItem);
            timingHoldUseItem = Time.time;       
        }
        if (context.canceled)
        {
            itemController.OnItemUsed(currentItem, Time.time - timingHoldUseItem);
            timingHoldUseItem = Time.time;
            isPressedThrow = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //Debug.LogFormat("Cx : Interact");
        if (context.started && interactor.currentInteractable != null)
        {
            interactor.currentInteractable.Interact(interactor);
        }
             
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.LogFormat("Cx : Direction is {0}", context.ReadValue<float>());
        movementDirection = (int)Math.Round(context.ReadValue<float>());
    }

    public void OnElevator(InputAction.CallbackContext context)
    {
        //Debug.LogFormat("Cx : Direction is {0}", context.ReadValue<float>());
        verticalMovementDirection = (int)Math.Round(context.ReadValue<float>());

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

    public void SetCurrentItem(ITEM_TYPE item)
    {
        currentItem = item;
        switch (currentItem)
        {
            case ITEM_TYPE.Hug:
                animator.SetTrigger("GetPlush");
                break;
            case ITEM_TYPE.Trumpet:
                animator.SetTrigger("GetTrumpet");
                break;
            case ITEM_TYPE.RedNose:
                animator.SetTrigger("GetRedNose");
                break;
            case ITEM_TYPE.Flower:
                animator.SetTrigger("GetFlower");
                break;
            case ITEM_TYPE.Gun:
                animator.SetTrigger("GetGun");
                break;
            case ITEM_TYPE.Ballon_Dog:
                animator.SetTrigger("GetBallon");
                break;
            case ITEM_TYPE.Ballon_Cringe:
                animator.SetTrigger("GetCringeBallon");
                break;
            case ITEM_TYPE.Ballon_Heart:
                animator.SetTrigger("GetHeartBallon");
                break;
            case ITEM_TYPE.Pie:
                animator.SetTrigger("GetPie");
                break;
            default:
                animator.SetTrigger("GetEmpty");
                break;
        }
    }
}
