using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Script used by the camera to avoid it from seeing behind the tilemap (follows the player and avoids exceeding map edges)
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Camera Elements")]
    [SerializeField] private GameObject _player;
    [Header("Map Limit Points")]
    [SerializeField] private float _yLowerLimit;
    [SerializeField] private float _yUpperLimit;
    [SerializeField] private float _xRightLimit;
    [SerializeField] private float _xLeftLimit;

    private void Update()
    {
        UpdatePosition();
    }

    private void OnValidate()
    {
        Assert.IsNotNull(_player);
    }

    private void UpdatePosition()
    {
        //Update the position :
        transform.position = new Vector3(
            Mathf.Clamp(_player.transform.position.x, _xLeftLimit, _xRightLimit),
            Mathf.Clamp(_player.transform.position.y, _yLowerLimit, _yUpperLimit),
            -6.5f);
    }
}
