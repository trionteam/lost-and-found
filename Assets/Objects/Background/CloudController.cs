using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    [SerializeField]
    private float _cloudSpeedMin = 0.05f;
    [SerializeField]
    private float _cloudSpeedMax = 0.2f;

    private float _cloudSpeed;

    public void Initialize(bool movingRight)
    {
        _cloudSpeed = Random.Range(_cloudSpeedMin, _cloudSpeedMax);
        if (!movingRight) _cloudSpeed = -_cloudSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * _cloudSpeed * Vector3.right;
    }
}
