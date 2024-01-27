using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ElevatorLocomotion : MonoBehaviour
{
    [SerializeField] float timing;
    public EmptyElevator currentEmpty;
    [SerializeField] EmptyElevator startEmptyElevator;
    public bool isMoving;

    private void Start()
    {
        currentEmpty = startEmptyElevator;
    }

    private void Update()
    {
        print(isMoving);
    }
    public void MoveToEmptyElevator(EmptyElevator target)
    {
        isMoving = true;
        StartCoroutine(SmoothLerp(target, 3f));
    }


    private IEnumerator SmoothLerp(EmptyElevator target, float time)
    {
        Vector3 startingPos = transform.position;
        Vector3 finalPos = target.transform.position;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
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
}

