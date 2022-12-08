using UnityEngine;
using Cinemachine;

public class CameraShake : Singleton<CameraShake>
{
    private CinemachineImpulseSource impulseSource;


    public void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float amplitudeGain = 1f, float duration = 0.5f, float power = 0.5f)
    {
        if(impulseSource != null)
        {
            impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitudeGain;
            impulseSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
            impulseSource.GenerateImpulse(new Vector3(Random.Range(-power, power), Random.Range(-power, power), Random.Range(-power, power)));
        }
    }
}
