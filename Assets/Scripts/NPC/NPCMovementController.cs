using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public enum NPC_MOVEMENT_STATE
{
    Idle,
    MoveRight,
    MoveLeft
}

/// <summary>
/// This script will be the one handling the movements of the NPCs
/// Do note that this movement is OPTIONAL
/// As such this script should only be added by the behaviour script upon order by the npc spawner. As such, NO SERIALIZE FIELDS HERE CHIEF-O
/// The only serialize field will contain a scriptable object containing the various elements required to move.
/// </summary>
public class NPCMovementController : MonoBehaviour
{
    private NPC_MOVEMENT_STATE _state;
    private Rigidbody2D _rb;
    private NPCMovementData _data;
    private int _curMoveCooldown;
    private Animator _anim;
    private bool _isFacingRight = true;

    private void Start()
    {
        _rb = gameObject.AddComponent<Rigidbody2D>();
        _state = NPC_MOVEMENT_STATE.Idle;
        StartCoroutine(PauseMovement());
    }

    public void InitializeData(NPCMovementData data, Animator anim)
    {
        _data = data;
        _anim = anim;
    }

    private IEnumerator PauseMovement()
    {
        _anim.SetBool("IsMoving", false);
        yield return new WaitForSeconds(_data.PauseTimer);
        _state = ChooseMovement();
        _curMoveCooldown = 0;
        StartCoroutine(Move());
    }

    /// <summary>
    /// The main coroutine handling the movement of the NPC
    /// (VERY UGLY BEWARE)
    /// How it works :
    /// - deactivates the rb (IsKinematic -> yes)
    /// - calculates new position depending on the move state (right or left)
    /// - if position is reachable -> move to it (otherwise, go back to idle)
    /// - call the coroutine again while increasing the cooldown.
    /// - when the latter reaches max value, NPC goes back to Idle
    /// </summary>
    /// <returns></returns>
    private IEnumerator Move()
    {
        yield return new WaitForFixedUpdate();
        _anim.SetBool("IsMoving",true);
        _rb.isKinematic = true;
        Vector2 newPos;
        if (_state == NPC_MOVEMENT_STATE.MoveRight)
        {
            newPos = transform.position + transform.right * Time.deltaTime*_data.Speed;
        }
        else
        {
            newPos = transform.position - transform.right * Time.deltaTime * _data.Speed;
        }
        if (!MovementIsPossible(newPos))
        {
            _state = NPC_MOVEMENT_STATE.Idle;
            StartCoroutine(PauseMovement());
        }
        else
        {
            transform.position = newPos;
            _curMoveCooldown++;
            if (_curMoveCooldown >= _data.MovementCooldown)
            {
                _state = NPC_MOVEMENT_STATE.Idle;
                StartCoroutine(PauseMovement());
            }
            else
            {
                StartCoroutine(Move());
            }
        }
    }

    /// <summary>
    /// Tests wether or not a movement is possible
    /// </summary>
    /// <param name="newPos">New calculated position</param>
    private bool MovementIsPossible(Vector2 newPos)
    {
        if (Physics2D.OverlapCircle(newPos, 0.1f, LayerMask.GetMask("ColliderTilemap")))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// The next movement after a pause is random (50-50)
    /// </summary>
    /// <returns></returns>
    private NPC_MOVEMENT_STATE ChooseMovement()
    {
        if (Random.Range(0, 100) < 50)
        {
            if(!_isFacingRight)
            {
                _isFacingRight = true;
                Flip();
            }
            return NPC_MOVEMENT_STATE.MoveRight;
        }
        else
        {
            if (_isFacingRight)
            {
                _isFacingRight = false;
                Flip();
            }
            return NPC_MOVEMENT_STATE.MoveLeft;
        }
    }

    private void Flip()
    {
        _anim.gameObject.GetComponent<SpriteRenderer>().flipX = !_anim.gameObject.GetComponent<SpriteRenderer>().flipX;
    } 

}
