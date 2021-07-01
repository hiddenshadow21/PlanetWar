using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Meteorite : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChange))]
    public Color color;
    float rotateScale;

    public void OnColorChange(Color _old, Color _new)
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_Color", _new);
    }

    private void Start()
    {
        rotateScale = Random.Range(50, 500);
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotateScale * Time.deltaTime));
    }
}
