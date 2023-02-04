using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EffectObject : MonoBehaviour
{
    [SerializeField]
    ParticleSystem particleSystem;

    public void SetStartColor(Color color) 
    {
        if (particleSystem == null)
            return;

        MainModule main = particleSystem.main;
        main.startColor = color;
    }

    private void OnParticleSystemStopped()
    {
        Destroy(gameObject);
    }
}
