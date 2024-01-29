using UnityEngine;

/// <summary>
/// Script that will handle the Area-Of-Effect a satisfied npc.
/// This area of effect will boost the speed of the player in proximity
/// </summary>
public class AOEHappiness : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Player player)) 
        {
            player.AddSpeedBoost(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.AddSpeedBoost(false);
        }
    }
}
