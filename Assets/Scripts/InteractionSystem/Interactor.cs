using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Interactor : MonoBehaviour
{
    [SerializeField] Transform _interactionPoint;
    [SerializeField] float _interactionRadius;
    [SerializeField] LayerMask _interactableMask;
    ContactFilter2D npcFilter;
    ContactFilter2D interactableFilter;

    Collider2D _collider;
    public IInteractable currentInteractable;

    // Update is called once per frame
    void Update()
    {

        _collider = Physics2D.OverlapCircle(new Vector2(_interactionPoint.position.x, _interactionPoint.position.y), _interactionRadius, _interactableMask);
        if (_collider != null)
        {
            _collider.TryGetComponent(out currentInteractable);
        } 
        else 
        {
            currentInteractable = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_interactionPoint.position, _interactionRadius);
        if (_collider)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _collider.transform.position);
        }
    }
}
