using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ElevatorLocomotion : MonoBehaviour
{
    [SerializeField] float timing = 3f;
    public EmptyElevator currentEmpty;
    [SerializeField] EmptyElevator startEmptyElevator;
    public bool isMoving;
    bool playerIsIn;
    [SerializeField] SpriteRenderer spriteUpArrow;
    [SerializeField] SpriteRenderer spriteDownArrow;

    private void Start()
    {
        currentEmpty = startEmptyElevator;
        spriteDownArrow.enabled = false;
        spriteUpArrow.enabled = false;
    }
    public void MoveToEmptyElevator(EmptyElevator target)
    {
        isMoving = true;
        spriteUpArrow.enabled = false;
        spriteDownArrow.enabled = false;
        StartCoroutine(StartMoving(target));
    }

    private IEnumerator StartMoving(EmptyElevator target)
    {
        Vector3 startingPos = transform.position;
        Vector3 finalPos = target.transform.position;
        float elapsedTime = 0;
        while (elapsedTime < timing)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / timing));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ArriveToDestination(target);      
    }
    private void ArriveToDestination(EmptyElevator target)
    {
        currentEmpty = target;
        isMoving = false;
    }

    public void UseElevator(bool goUp)
    {
        Debug.Log("use");
        if (goUp) {
            if (currentEmpty.upNeighbor != null)MoveToEmptyElevator(currentEmpty.upNeighbor);
        }
        else { 
            if (currentEmpty.downNeighbor != null) MoveToEmptyElevator(currentEmpty.downNeighbor); 
        }
    }

    public void PlayerEnter(Player player)
    {
        Debug.Log("enter elevator");
        playerIsIn = true;
        player.EnterInElevator(this);
        if (currentEmpty.upNeighbor != null) spriteUpArrow.enabled = true;
        if (currentEmpty.downNeighbor != null) spriteDownArrow.enabled = true;

        
    }

    public void QuitElevator()
    {
        Debug.Log("quit elevator");
        playerIsIn = false;
        spriteUpArrow.enabled = false;
        spriteDownArrow.enabled = false;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!playerIsIn && !isMoving && collision.gameObject.GetComponent<Player>()) { 
            PlayerEnter(collision.gameObject.GetComponent<Player>()); 
            print("Enter in Elevator"); 
        }
    }
}

