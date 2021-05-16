using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Meteorite : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChange))]
    public Color color;

    public void OnColorChange(Color _old, Color _new)
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_Color", _new);
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 50F * Time.deltaTime));
    }
}
