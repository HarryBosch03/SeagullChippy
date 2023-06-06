using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seagull : MonoBehaviour
{

    public ParticleSystem particles;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Hit();
    }

    public void Hit()
    {
        GameManager.instance.AddScore(1);
        particles.Play();

    }
}
