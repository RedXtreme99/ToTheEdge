using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform _objectToFollow = null;

    Vector3 _objectOffset;

    private void Awake()
    {
        // Determine offset between camera and player
        _objectOffset = this.transform.position - _objectToFollow.transform.position;
    }

    // Happens after Update(), camera moves last
    private void LateUpdate()
    {
        // Maintain offset between camera and player
        this.transform.position = _objectToFollow.position + _objectOffset;
    }
}
