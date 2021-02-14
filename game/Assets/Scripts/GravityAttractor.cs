using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    public Rigidbody2D rb;
    private static List<GravityAttractor> Attractors = new List<GravityAttractor>();
    private const float G = 6.67f;

    private void OnEnable()
    {
        if(!Attractors.Contains(this))
            Attractors.Add(this);
    }

    private void OnDisable()
    {
        if(Attractors.Contains(this))
            Attractors.Remove(this);
    }

    public static List<GravityAttractor> GetListOfAttractors()
    {
        return Attractors;
    }


    public void Attract(GravityBody gravityBody)
    {
        Rigidbody2D rbToAttract = gravityBody.rb;
        Vector2 direction = (Vector2)transform.position - rbToAttract.position;
        float distance = direction.magnitude;

        float force = G*(rb.mass * rbToAttract.mass) / (float)Math.Pow(distance, 2);
        rbToAttract.AddForce(force * direction.normalized);
    }
}
