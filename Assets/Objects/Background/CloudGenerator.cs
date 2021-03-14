using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    [SerializeField]
    private CloudController[] _prefabs = default;

    [SerializeField]
    private float _minCloudPeriod = 40.0f;
    [SerializeField]
    private float _maxCloudPeriod = 20.0f;

    [SerializeField]
    private int _numInitialClouds = 3;

    [SerializeField]
    private float _minCloudX = -13.0f;
    [SerializeField]
    private float _maxCloudX = 13.0f;
    [SerializeField]
    private float _minCloudY = -5.0f;
    [SerializeField]
    private float _maxCloudY = 5.0f;

    private float _nextCloudTime = 0;

    private void Start()
    {
        for (int i = 0; i < _numInitialClouds; ++i)
        {
            AddCloud(false);
        }
        _nextCloudTime = Time.time + Random.Range(_minCloudPeriod, _maxCloudPeriod);
    }

    private void Update()
    {
        if (_nextCloudTime > Time.time) return;
        AddCloud(true);
        _nextCloudTime = Time.time + Random.Range(_minCloudPeriod, _maxCloudPeriod);
    }

    private void AddCloud(bool startAtEdge)
    {
        float x;
        bool movingRight;
        if (startAtEdge)
        {
            movingRight = Random.Range(0, 2) == 0;
            x = movingRight ? _minCloudX : _maxCloudX;
        }
        else
        {
            x = Random.Range(_minCloudX, _maxCloudX);
            movingRight = x < 0.0f;
        }
        float y = Random.Range(_minCloudY, _maxCloudY);
        var cloudPrefab = _prefabs[Random.Range(0, _prefabs.Length)];
        var position = new Vector3(x, y, transform.position.z);
        var cloud = Instantiate(cloudPrefab, position, Quaternion.identity, transform);
        cloud.GetComponent<CloudController>().Initialize(movingRight);
    }
}
