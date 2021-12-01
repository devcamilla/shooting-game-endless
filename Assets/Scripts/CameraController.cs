using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerShipController _player;

    public GameObject stageObj;

    public float smoothing;

    public Vector2 bounds;

    void Update()
    {
        var targetPos = new Vector3(0, 0, transform.position.z);
        if (_player)
        {
            var x = Mathf.Clamp(_player.transform.position.x, -bounds.x, bounds.x);
            var y = Mathf.Clamp(_player.transform.position.y, -bounds.y, bounds.y);
            targetPos = new Vector3(x, y, transform.position.z);
        }
        else
        {
            _player = stageObj.GetComponentInChildren<PlayerShipController>();
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothing * Time.deltaTime);
    }
}
