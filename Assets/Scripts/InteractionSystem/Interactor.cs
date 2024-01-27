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

    void Awake()
    {
        // npcFilter.SetLayerMask(_npcMask);
        // interactableFilter.SetLayerMask(_interactableMask);
    }
    // Update is called once per frame
    void Update()
    {
        List<Collider2D> interactableColliders = new List<Collider2D>();

        if (Physics2D.OverlapCircle(new Vector2(_interactionPoint.position.x, _interactionPoint.position.y), _interactionRadius, interactableFilter, interactableColliders)>0)
        {
            _collider = interactableColliders.OrderBy(col => (col.transform.position - transform.position).magnitude).First();
            _collider.TryGetComponent(out currentInteractable);
        } else {
            currentInteractable = null;
        }

        // _collider = Physics2D.OverlapCircle(new Vector2(_interactionPoint.position.x, _interactionPoint.position.y), _interactionRadius, _interactableMask);
        // if (_collider != null)
        // {
        //     _collider.TryGetComponent(out currentInteractable);
        // }
    }

    public List<IInteractable> getInteractableNPCs()
    {
        List<Collider2D> npcColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(new Vector2(_interactionPoint.position.x, _interactionPoint.position.y), _interactionRadius, npcFilter, npcColliders);
        List<IInteractable> res = new List<IInteractable>();
        foreach (Collider2D npcCol in npcColliders){
            IInteractable tempInteractable;
            if (npcCol.TryGetComponent<IInteractable>(out tempInteractable)){
                res.Add(tempInteractable);
            }
        }
        return res;
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
