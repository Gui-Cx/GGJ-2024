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

    private void Update()
    {
        //print(isMoving);
    }
    public void MoveToEmptyElevator(EmptyElevator target)
    {
        isMoving = true;
        StartCoroutine(SmoothLerp(target));
    }


    private IEnumerator SmoothLerp(EmptyElevator target)
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
        spriteUpArrow.enabled = false;
        spriteDownArrow.enabled = false;
        if (goUp) ArriveToDestination(currentEmpty.upNeighbor);
        else ArriveToDestination(currentEmpty.downNeighbor);
    }

    public void PlayerEnter()
    {
        playerIsIn = true;
        if (currentEmpty.upNeighbor != null) spriteUpArrow.enabled = true;
        if (currentEmpty.downNeighbor != null) spriteDownArrow.enabled = true;
    }

    void QuitElevator()
    {
        playerIsIn = false;
        spriteUpArrow.enabled = false;
        spriteDownArrow.enabled = false;
        Debug.Log("quit");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>()) { PlayerEnter(); print("Enter in Elevator"); }
    }
}

