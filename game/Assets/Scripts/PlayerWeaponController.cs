﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour
{
    private Transform aimTransform;

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        HandleAiming();
    }

    private void HandleAiming()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 aimLocalScale = Vector3.one;
        if(Quaternion.Angle(transform.rotation, aimTransform.rotation) > 90)
        {
            aimLocalScale.y = -1f;
        }
        else
        {
            aimLocalScale.y = 1f;
        }
        aimTransform.localScale = aimLocalScale;
    }
}

