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
    private Player _player;
    private float _playerButtonPosition;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerButtonPosition = _button.transform.localPosition.x;

        SetUIState(false);
    }

    // Update is called once per frame
    void Update()
    {
        _collider = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), _interactionRadius, _interactableMask);
        
        if (_collider != null)
        {
            _collider.TryGetComponent(out currentInteractable);
            UIController.Instance.SetInteractButton(true);
        }
        else 
        {
            currentInteractable = null; 
            UIController.Instance.SetInteractButton(false);
        }
    }

    private void SetUIState(bool active)
    {
        UIController.Instance.SetInteractButton(active);
        _button.SetActive(active);

        float buttonPosition = _player.IsFacingRight ? _playerButtonPosition : -_playerButtonPosition;
        _button.transform.localPosition.Set(buttonPosition, transform.localPosition.y, transform.localPosition.z);
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
