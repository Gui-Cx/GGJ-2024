using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Script that will handle the Area-Of-Effect a satisfied npc.
/// This area of effect will boost the speed of the player in proximity
/// </summary>
public class AOEHappiness : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    [Header("AOE Elements")]
    [SerializeField] private int _speedMultiplicativeModifier=2;

    private int _prevPlayerSpeed;
    private bool _isBoostingPlayer=false;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Player>()!=null && collision.gameObject.GetComponent<Player>().IsBoostedByHappinessAOE == false) 
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.IsBoostedByHappinessAOE = true;
            _prevPlayerSpeed = player.speed;
            player.speed = player.speed * 2;
            _isBoostingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null && _isBoostingPlayer) //if the happiness AOE sees the player fleeing AND it was boosting its speed, then it sets it back
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.IsBoostedByHappinessAOE = false;
            player.speed = _prevPlayerSpeed;
            _isBoostingPlayer = false;
        }
    }
}
