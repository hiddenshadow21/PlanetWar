using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : NetworkBehaviour
{
    public Rigidbody2D rb;
    private List<GravityAttractor> attractors;
    void Start()
    {
        attractors = GravityAttractor.GetListOfAttractors();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        foreach (var attractor in attractors)
        {
            attractor.Attract(this);
        }
    }
}
