using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager Instance { private set; get; }
    private void Awake() => Instance = this;

    [Header("Create")]
    [SerializeField] private GameObject  planetPrefab;
    [SerializeField] private float       range;
    
    [Header("Management")]
    [SerializeField] private List<Planet> planets;
    [SerializeField] private float        massPerDist;
    [SerializeField] private float        gravityConst = 0.1f;

    [Header("UI")]
    [SerializeField] private Scrollbar speedScrollbar;

    private List<Planet> planetsDead = new List<Planet>();

    public List<Planet> Planets { get => planets; }


    private void LateUpdate()
    {
        // 행성간 중력 작용
        foreach ( Planet planet in planets )
        {
            List<Planet> others = planets.FindAll(x => x != planet);
            foreach ( Planet other in others )
            {
                Vector3 direction = (other.transform.position - planet.transform.position).normalized;
                float   distance  = Vector3.Distance(planet.transform.position, other.transform.position);
                if ( (distance <= planet.transform.localScale.x/2) || (distance <= other.transform.localScale.x/2) )
                {
                    PlanetCollision(planet, other);
                }
                if ( distance >= massPerDist )
                {
                    planet.transform.Translate(direction * gravityConst * other.PlanetMass / Mathf.Pow(distance, 2) *
                        Mathf.Pow(speedScrollbar.value*100, 3));
                }
            }
        }

        // 사망한 행성 처리
        planetsDead.ForEach(x => planets.Remove(x));
        planetsDead.ForEach(x => x.gameObject.SetActive(false));
    }

    /// <summary>
    /// 두 행성이 충돌할 경우 호출
    /// </summary>
    private void PlanetCollision(Planet planet1, Planet planet2)
    {
        // 행성1이 행성2에 비해 질량이 몹시 큰 경우
        if ( planet1.PlanetMass >= planet2.PlanetMass*1.5f )
        {
            planet1.PlanetExtend(planet2.PlanetMass * 0.7f);
            planetsDead.Add(planet2);
            EffectPool.Instance.CrashEffect(planet2.transform.position, planet2.transform.localScale.x / 2);

            if ( planet2.transform == ControlPanel.Instance.Target ) { ControlPanel.Instance.SetTarget(planet1.transform); }

            Debug.Log(planet1.PlanetName + " eats " + planet2.PlanetName);
        }
        // 행성2가 행성1에 비해 질량이 몹시 큰 경우
        else if ( planet2.PlanetMass >= planet1.PlanetMass*1.5f )
        {
            planet2.PlanetExtend(planet1.PlanetMass * 0.7f);
            planetsDead.Add(planet1);
            EffectPool.Instance.CrashEffect(planet1.transform.position, planet1.transform.localScale.x / 2);

            if ( planet1.transform == ControlPanel.Instance.Target ) { ControlPanel.Instance.SetTarget(planet2.transform); }

            Debug.Log(planet2.PlanetName + " eats " + planet1.PlanetName);
        }
        // 두 행성간 질량이 비슷한 경우: 모두 소멸
        else
        {
            planetsDead.Add(planet1);
            planetsDead.Add(planet2);
            Planet bigger = planet1.PlanetMass > planet2.PlanetMass ? planet1 : planet2;
            EffectPool.Instance.CrashEffect(bigger.transform.position, bigger.transform.localScale.x / 2);

            if ( (planet1.transform == ControlPanel.Instance.Target) || (planet2.transform == ControlPanel.Instance.Target) )
            {
                ControlPanel.Instance.SetTarget(null);
            }
        }
    }

    /// <summary>
    /// 새로운 행성 생성
    /// </summary>
    public void NewPlanet(string _name, float _radius, float _mass)
    {
        GameObject planet;
        if ( planetsDead.Count != 0 )
        {
            Planet dead = planetsDead[0];
            planetsDead.Remove(dead);
            planet = dead.gameObject;
            planet.SetActive(true);
        }
        else
        {
            planet = Instantiate(planetPrefab, transform);
        }
        planet.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
        planet.GetComponent<Planet>().Setup(_name, _mass);
        planet.transform.position   = Random.insideUnitSphere * range;
        planet.transform.localScale = Vector3.one * _radius * 2;

        planets.Add(planet.GetComponent<Planet>());
    }
}
