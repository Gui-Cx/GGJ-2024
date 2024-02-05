using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _interactionRadius;
    [SerializeField] Vector2 _interactorOffset;
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
        Vector2 interactorPosition = (Vector2) transform.position + _interactorOffset;
        _collider = Physics2D.OverlapCircle(interactorPosition, _interactionRadius, _interactableMask);
        
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
        Vector2 interactorPosition = (Vector2)transform.position + _interactorOffset;
        Gizmos.DrawSphere(interactorPosition, _interactionRadius);
        if (_collider)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(interactorPosition, _collider.transform.position);
        }
    }
}
