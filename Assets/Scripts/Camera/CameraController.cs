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
    [SerializeField] private float _xMin;
    [SerializeField] private float _yMin;
    [SerializeField] private float _xMax;
    [SerializeField] private float _yMax;

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
            Mathf.Clamp(_player.transform.position.x, _xMin, _xMax),
            Mathf.Clamp(_player.transform.position.y, _yMin, _yMax),
            -6.5f);
    }
}
