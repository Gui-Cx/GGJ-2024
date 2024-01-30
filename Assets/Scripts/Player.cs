using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerState
{
    Idle,
    InElevator
}

public class Player : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _happynessSpeedBoost;

    private Rigidbody2D rigidbody2d;
    private Interactor interactor;
    private ItemController itemController;
    private Animator animator;
    private ElevatorLocomotion currentElevator;
    private Transform _currentElevatorPoition; //contains the correct transform the position the player

    private PlayerState _currentState;
    private ITEM_TYPE _currentItem;
    private Dictionary<ITEM_TYPE, ParticleSystem> _itemParticles;

    private float _currentSpeed;
    private int _movementDirection = 0;
    private int _verticalMovementDirection = 0;
    private bool _isFacingRight = true;
    private bool _isBoosted = false;

    private float _timingHoldUseItem;
    private bool _isPressedThrow;

    public bool IsFacingRight => _isFacingRight;
    public ITEM_TYPE CurrentItem => _currentItem;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        interactor = GetComponent<Interactor>();
        animator = GetComponent<Animator>();
        itemController = GetComponent<ItemController>();
    }

    private void Start()
    {
        _currentState = PlayerState.Idle;
        _currentItem = ITEM_TYPE.None;
        _itemParticles = new Dictionary<ITEM_TYPE, ParticleSystem>();

        UIController.Instance.SetItemButton(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (_currentState)
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
                transform.position = _currentElevatorPoition.position;
                if (_movementDirection != 0) { TryQuitElevator(_movementDirection); }
                if (_verticalMovementDirection != 0) { TryUseElevator(_verticalMovementDirection); }
                break;
        }
        if(_isPressedThrow) itemController.GetTimeHold(Time.time - _timingHoldUseItem);

    }

    public void AddSpeedBoost(bool active)
    {
        _isBoosted = active;
    }

    void Flip()
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = !gameObject.GetComponent<SpriteRenderer>().flipX;
        _isFacingRight=!_isFacingRight;
    }
    
    public void PlayParticles()
    {
        if (!_itemParticles.TryGetValue(_currentItem, out ParticleSystem particles))
        {
            GameObject particlePrefab = GameManager.Instance.ItemsData.Items.First(item => item.Type == _currentItem).Particles;
            particles = Instantiate(particlePrefab, transform).GetComponent<ParticleSystem>();
            print("Instantiate " + _currentItem);
            _itemParticles.Add(_currentItem, particles);
        }
        if (!_isFacingRight) particles.transform.localEulerAngles = new Vector3(0, 180, 0);
        particles.GetComponent<ParticleSystem>().Play();
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (_currentItem != ITEM_TYPE.None && _currentItem != ITEM_TYPE.Pie){
            PlayParticles();
        }
        if (context.started)
        {
            _isPressedThrow = true;
            itemController.OnItemUsed(_currentItem);
            _timingHoldUseItem = Time.time;       
        }
        if (context.canceled)
        {
            itemController.OnItemUsed(_currentItem, Time.time - _timingHoldUseItem);
            _timingHoldUseItem = Time.time;
            _isPressedThrow = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //Debug.LogFormat("Cx : Interact");
        if (context.started && interactor.currentInteractable != null && _currentState ==PlayerState.Idle)
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

    public void EnterInElevator(ElevatorLocomotion elevator, Transform elevatorPosition)
    {
        _currentState = PlayerState.InElevator;
        currentElevator = elevator;
        _currentElevatorPoition = elevatorPosition;
        GetComponent<BoxCollider2D>().enabled = false;
        rigidbody2d.simulated = false;
        transform.position = elevatorPosition.position;
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
            _currentState = PlayerState.Idle;
            GetComponent<BoxCollider2D>().enabled = true;
            //if (movementDirection < 0) transform.position +=  new Vector3(-1f,0,0);
            //else transform.position += new Vector3(1f, 0, 0);
            rigidbody2d.simulated = true;
            currentElevator.QuitElevator();
        }
    }

    public void SetCurrentItem(ITEM_TYPE item)
    {
        if (item == _currentItem) _currentItem = ITEM_TYPE.None;
        else _currentItem = item;

        switch (_currentItem)
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

        UIController.Instance.SetItemButton(_currentItem != ITEM_TYPE.None);
    }
}
