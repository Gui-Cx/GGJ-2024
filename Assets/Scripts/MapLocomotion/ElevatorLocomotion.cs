using System.Collections;
using UnityEngine;

public class ElevatorLocomotion : MonoBehaviour, IInteractable
{
    [SerializeField] private float timing;
    [SerializeField] private EmptyElevator startPosition;
    [SerializeField] private SpriteRenderer spriteUpArrow;
    [SerializeField] private SpriteRenderer spriteDownArrow;
    [SerializeField] private Transform _playerPosition;

    [HideInInspector] public EmptyElevator currentEmpty;
    [HideInInspector] public bool isMoving;
    private bool playerIsIn;

    private void Start()
    {
        SetCurrentEmpty(startPosition);
        DisplayArrows(false);
    }

    public void MoveToEmptyElevator(EmptyElevator target)
    {
        isMoving = true;
        DisplayArrows(false);
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.TravelingElevator, this.transform.position);
        StartCoroutine(StartMoving(target));
    }

    void DisplayArrows(bool isDisplay)
    {
        if (isDisplay)
        {
            if (currentEmpty.upNeighbor != null) spriteUpArrow.enabled = true;
            if (currentEmpty.downNeighbor != null) spriteDownArrow.enabled = true;
        }
        else {
            spriteUpArrow.enabled = false;
            spriteDownArrow.enabled = false;
        }
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
            yield return new WaitForFixedUpdate();
        }
        ArriveToDestination(target);      
    }
    private void ArriveToDestination(EmptyElevator target)
    {
        SetCurrentEmpty(target);
        isMoving = false;
        if (playerIsIn) DisplayArrows(true);
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.ElevatorArrives, this.transform.position);
    }


    public void SetCurrentEmpty(EmptyElevator target)
    {
        if(currentEmpty) currentEmpty.Collider.enabled = true;
        target.Collider.enabled = false;
        currentEmpty = target;
    }

    public void UseElevator(bool goUp)
    {
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
        player.EnterInElevator(this, _playerPosition);
        DisplayArrows(true);
    }

    public void QuitElevator()
    {
        Debug.Log("quit elevator");
        playerIsIn = false;
        DisplayArrows(false);
    }

    public bool Interact(Interactor interactor)
    {
        if (!playerIsIn && !isMoving )
        {
            PlayerEnter(interactor.gameObject.GetComponent<Player>());
            return true;
        }
        return false;
    }
}

