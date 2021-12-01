using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameState;

public class BulletController : MonoBehaviour
{
    private float _speed;

    private bool _spinning;

    private float _targetX;

    public float directionY;

    public float lifetime;

    void Update()
    {
        transform.position += new Vector3(_targetX, directionY, 0) * _speed * Time.deltaTime;

        if (_spinning)
            transform.Rotate(new Vector3(0, 0, 90 * _speed * Time.deltaTime));

        lifetime -= Time.deltaTime;
        if (lifetime < 0)
            Destroy(gameObject);
    }

    public void SetBullet(Bullet bullet)
    {
        _speed = bullet.speed;
        _spinning = bullet.spinning;

        if (bullet.accuracy > 0)
        {
            var player = transform.parent.GetComponentInChildren<PlayerShipController>();
            if (player)
            {
                var direction = (player.transform.position - transform.position).normalized;
                _targetX = direction.x * bullet.accuracy;
            }
        }

        var renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = bullet.texture;

        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = bullet.audioClip;
        audioSource.Play();
    }
}
