using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _interactionRadius;
    [SerializeField] LayerMask _interactableMask;

    [Header("Player UI")]
    [SerializeField] private GameObject _button;

    public IInteractable currentInteractable;
    private Collider2D _collider;

    private void Start()
    {
        SetUIState(false);
    }

    // Update is called once per frame
    void Update()
    {
        _collider = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), _interactionRadius, _interactableMask);
        
        if (_collider != null)
        {
            _collider.TryGetComponent(out currentInteractable);
            SetUIState(true);
        }
        else 
        {
            currentInteractable = null;
            SetUIState(false);
        }
    }

    private void SetUIState(bool active)
    {
        UIController.Instance.SetInteractButton(active);
        _button.SetActive(active);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, _interactionRadius);
        if (_collider)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _collider.transform.position);
        }
    }
}
