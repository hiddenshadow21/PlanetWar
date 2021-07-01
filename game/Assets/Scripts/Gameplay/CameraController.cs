using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    public GameObject back1;
    public GameObject back2;
    public GameObject back3;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player == null)
            return;
        LockCameraToPlayer();
    }

    private void LockCameraToPlayer()
    {
        var playerPos = player.transform.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, playerPos, ref velocity, smoothSpeed);
        var pos = transform.position;
        pos.z = -10;
        transform.position = pos;
        back1.transform.position = new Vector3(pos.x / 1.9f, pos.y / 1.9f, 2);
        back2.transform.position = new Vector3(pos.x / 1.6f, pos.y / 1.6f, 3);
        back3.transform.position = new Vector3(pos.x / 1.3f, pos.y / 1.3f, 4);
    }

    public IEnumerator ShakeCamera(float power, float length)
    {
        float tick = 40 * length;

        while (tick != 0)
        {
            transform.position = transform.position + UnityEngine.Random.insideUnitSphere * power;
            tick--;

            if(tick < 40 * length * 0.5)
                power *= 0.9f;

            yield return new WaitForSeconds(0.025f);
        }
    }
}
