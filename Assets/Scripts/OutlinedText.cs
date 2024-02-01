using TMPro;
using UnityEngine;

public class OutlinedText : MonoBehaviour
{
    private TextMeshProUGUI _baseText;
    private TextMeshProUGUI _outlinetext;

    private void Awake()
    {
        _baseText = GetComponent<TextMeshProUGUI>();
        _outlinetext = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string value)
    {
        _baseText.text = value;
        _outlinetext.text = value;
    }
}
