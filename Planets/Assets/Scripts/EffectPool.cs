using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance { private set; get; }
    private void Awake() => Instance = this;

    [SerializeField] private GameObject       crashEffectPrefab;
    [SerializeField] private List<GameObject> crashEffects;

    /// <summary>
    /// 해당지점에 Effect 생성
    /// </summary>
    public void CrashEffect(Vector3 position, float radius)
    {
        GameObject effect = crashEffects.Find(x => !x.activeSelf);
        if ( effect == null )
        {
            effect = Instantiate(crashEffectPrefab, transform);
            crashEffects.Add(effect);
        }
        effect.transform.position   = position;
        effect.transform.localScale = Vector3.one * radius / 10;
        effect.SetActive(true);
    }
}
