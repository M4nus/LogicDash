using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Insanity : MonoBehaviour
{
    private AudioSource audioSource1;
    private AudioSource audioSource2;
    public float pitch1 = 1f;
    public float pitch2 = 1f;

    public Transform playerPosition;
    public Transform pos1;
    public Transform pos2;

    public Volume postProcessVolume;

    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberation;
    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        audioSource1 = AudioManager.Instance.musicSources[0];
        audioSource2 = AudioManager.Instance.musicSources[1];
        postProcessVolume.profile.TryGet(out lensDistortion);
        postProcessVolume.profile.TryGet(out chromaticAberation);
        postProcessVolume.profile.TryGet(out colorAdjustments);
    }

    void Update()
    {
        float blend = Mathf.InverseLerp(pos1.position.x + 15f, pos2.position.x, playerPosition.position.x);
        blend = Mathf.Clamp(blend, 0f, 1f);
        audioSource1.pitch = Mathf.Lerp(pitch1, pitch2, blend);
        audioSource2.pitch = Mathf.Lerp(pitch1, pitch2, blend);

        float dist1 = Vector3.Distance(playerPosition.position, pos1.position);
        float dist2 = Vector3.Distance(playerPosition.position, pos2.position);

        float blendRatio = dist1 / (dist1 + dist2);
        
        lensDistortion.intensity.value = Mathf.Lerp(-0.1f, -0.8f, blendRatio);
        chromaticAberation.intensity.value = Mathf.Lerp(0.1f, 1f, blendRatio);
        colorAdjustments.hueShift.value = Mathf.Lerp(0f, 180f, blendRatio);
    }
}