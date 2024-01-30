using System.Collections;
using UnityEngine;

public class Slide : MonoBehaviour, IInteractable
{
    [SerializeField] private float _slideMaxSpeed;
    [SerializeField] private float _timeToReachMaxSpeed;
    [SerializeField] private Transform _endPosition;

    private bool _isSliding = false;

    public bool Interact(Interactor interactor)
    {
        if (!_isSliding && interactor.TryGetComponent<Player>(out var player))
        {
            player.StartSlide();
            StartCoroutine(SlideMovement(player, interactor.transform));
            return true;
        }
        return false;
    }

    private IEnumerator SlideMovement(Player player, Transform transform)
    {
        float slideTime = 0;
        float slideSpeed = 0;

        transform.position = this.transform.position;
        
        while (transform.position.y > _endPosition.position.y)
        {
            yield return new WaitForEndOfFrame();

            slideTime += Time.deltaTime;
            slideSpeed = Mathf.Lerp(0, _slideMaxSpeed, slideTime / _timeToReachMaxSpeed);

            transform.position += Vector3.down * slideSpeed;
        }

        transform.position = _endPosition.position;
        player.EndSlide();
    }
}
