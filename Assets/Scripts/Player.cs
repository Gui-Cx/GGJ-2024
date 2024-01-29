using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerState
{
    Idle,
    InElevator
}

[System.Serializable]
public class ItemParticleSystem
{
    public ITEM_TYPE itemType;
    public ParticleSystem itemParticleSystem;
}


public class Player : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _happynessSpeedBoost;

    [Header("Particles")]
    [SerializeField] private GameObject particleParent;
    [SerializeField] private ItemParticleSystem[] itemParticleSystems;

    private Rigidbody2D rigidbody2d;
    private Interactor interactor;
    private ItemController itemController;
    private Animator animator;
    private ElevatorLocomotion currentElevator;

    private PlayerState currentState;
    private ITEM_TYPE currentItem;

    private float _currentSpeed;
    private int _movementDirection = 0;
    private int _verticalMovementDirection = 0;
    private bool _isFacingRight = true;
    private bool _isBoosted = false;

    private float timingHoldUseItem;
    private bool isPressedThrow;

    public bool IsFacingRight => _isFacingRight;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        interactor = GetComponent<Interactor>();
        animator = GetComponent<Animator>();
        itemController = GetComponent<ItemController>();

        currentState = PlayerState.Idle;
    }    

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                _currentSpeed = _isBoosted ? _maxSpeed * _happynessSpeedBoost : _maxSpeed;
                rigidbody2d.AddForce(_movementDirection * _currentSpeed * Vector2.right);

                if (_movementDirection != 0)
                {
                    animator.SetBool("isMoving", true);
                    if (_movementDirection > 0 && !_isFacingRight) Flip();
                    if (_movementDirection < 0 && _isFacingRight) Flip();
                }
                else animator.SetBool("isMoving", false);
                break;

            case PlayerState.InElevator:
                transform.position = currentElevator.transform.position;
                if (_movementDirection != 0) { TryQuitElevator(_movementDirection); }
                if (_verticalMovementDirection != 0) { TryUseElevator(_verticalMovementDirection); }
                break;
        }
        if(isPressedThrow) itemController.GetTimeHold(Time.time - timingHoldUseItem);

    }

    public void AddSpeedBoost(bool active)
    {
        _isBoosted = active;
    }

    void Flip()
    {
        particleParent.transform.Rotate(new Vector3(0, 180, 0));
        gameObject.GetComponent<SpriteRenderer>().flipX = !gameObject.GetComponent<SpriteRenderer>().flipX;
        _isFacingRight=!_isFacingRight;
    }
    
    public void PlayParticles(){
        ItemParticleSystem correspondingItemParticleSystem = itemParticleSystems.First(item => item.itemType == currentItem);
        correspondingItemParticleSystem?.itemParticleSystem?.Play();
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (currentItem != ITEM_TYPE.Pie){
            PlayParticles();
        }        
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
        if (context.started && interactor.currentInteractable != null && currentState ==PlayerState.Idle)
        {
            interactor.currentInteractable.Interact(interactor);
        }
             
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.LogFormat("Cx : Direction is {0}", context.ReadValue<float>());
        _movementDirection = (int)Math.Round(context.ReadValue<float>());
    }

    public void OnElevator(InputAction.CallbackContext context)
    {
        //Debug.LogFormat("Cx : Direction is {0}", context.ReadValue<float>());
        _verticalMovementDirection = (int)Math.Round(context.ReadValue<float>());

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
            //if (movementDirection < 0) transform.position +=  new Vector3(-1f,0,0);
            //else transform.position += new Vector3(1f, 0, 0);
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
