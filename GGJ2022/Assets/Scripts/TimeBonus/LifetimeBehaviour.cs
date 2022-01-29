using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifetimeBehaviour : MonoBehaviour
{
    private float lifespan;
    
    public float GetLifespan()
    {
        return lifespan;
    }
    public void SetLifespan(float time)
    {
        lifespan = time;
    }

    private void FixedUpdate()
    {
        lifespan -= Time.fixedDeltaTime;

        if (lifespan <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
