using UnityEngine;

public class EffectAutoDisable : MonoBehaviour
{
    private void Update() => gameObject.SetActive(GetComponent<ParticleSystem>().isPlaying);
}
