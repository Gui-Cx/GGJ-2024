using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] Transform _interactionPoint;
    [SerializeField] float _interactionRadius;
    [SerializeField] LayerMask _interactableMask;

    Collider2D _collider;

    // Update is called once per frame
    void Update()
    {
        _collider = Physics2D.OverlapCircle(new Vector2(_interactionPoint.position.x, _interactionPoint.position.y), _interactionRadius, _interactableMask);
        
        IInteractable interactable = null;

        
        if (_collider != null && _collider.TryGetComponent(out interactable)
            // && Keyboard.current.eKey.wasPressedThisFrame -> player press E
            )
            interactable.Interact(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_interactionPoint.position, _interactionRadius);
    }
}