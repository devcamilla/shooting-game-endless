using System.Collections;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private float _elapsedMoveTime;

    private float _targetRotX;

    public float speed;

    public float moveTime;

    private void Start()
    {
        _targetRotX = Random.value > .5f ? -1 : 1;
        _elapsedMoveTime = moveTime;
    }

    private void Update()
    {
        transform.position += new Vector3(_targetRotX, -.5f) * speed * Time.deltaTime;
        if (_elapsedMoveTime < 0)
        {
            _targetRotX = -_targetRotX;
            _elapsedMoveTime = moveTime;
        }
        _elapsedMoveTime -= Time.deltaTime;
    }
}
