using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


[Flags]
public enum EulerAxis
{
    X,
    Z
}

public class BlockMovement : MonoBehaviour
{
    public EulerAxis axis;

    [SerializeField] private float timePeriod = 2; // in seconds
    [SerializeField] private float blockMomentum = 1.5f;

    private GameController _gameController;
    private float _maxAxisLimit;
    private Vector3 _startPosition;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        _gameController =
            GameObject.FindWithTag("GameController")?.GetComponent(typeof(GameController)) as GameController;
        _maxAxisLimit = _gameController.maxLimitBlock;
        _startPosition = transform.position;
        startTime = Time.fixedTime;
    }

    void FixedUpdate()
    {
        //  Pendulum Motion of the block
        //      Taken from basic physics formula of x(t) = Dsin(a + wt)
        transform.position = axis switch
        {
            EulerAxis.X =>  _startPosition + transform.right *
                (float)(_maxAxisLimit * Math.Sin((2 * Math.PI / timePeriod) * (Time.fixedTime - startTime))),
            EulerAxis.Z => _startPosition + transform.forward *
                (float)(_maxAxisLimit * Math.Sin((2 * Math.PI / timePeriod) * (Time.fixedTime - startTime))),
            _ => transform.position
        };
        
        
    }
}