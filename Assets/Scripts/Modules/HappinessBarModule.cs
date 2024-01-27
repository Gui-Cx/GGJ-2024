using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script contained by the happiness bar object, to be modified by the NPC scripts (NPCHappinessBarController)
/// </summary>
public class HappinessBarModule : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }
    public void SetMaxHappiness(int val)
    {
        _slider.maxValue = val;
        _slider.value = val;
    }
    public void SetHappinessValue(int val)
    {
        _slider.value = val;
    }
}
