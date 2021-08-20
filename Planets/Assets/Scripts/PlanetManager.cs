using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager Instance { private set; get; }
    private void Awake() => Instance = this;

    [Header("Create")]
    [SerializeField] private GameObject  planetPrefab;
    [SerializeField] private float       range;
    [SerializeField] private float       minMass;
    [SerializeField] private float       maxMass;
    [SerializeField] private float       minRadius;
    [SerializeField] private float       maxRadius;
    
    [Header("Management")]
    [SerializeField] private List<Planet> planets;
    [SerializeField] private float        massPerDist;
    [SerializeField] private float        gravityConst = 0.01f;

    private List<Planet> planetsToDie = new List<Planet>();

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
                    planet.transform.Translate(direction * gravityConst * planet.PlanetMass * other.PlanetMass / Mathf.Pow(distance, 2));
                }
            }
        }

        // 사망한 행성 처리
        foreach ( Planet planet in planetsToDie )
        {
            planets.Remove(planet);
            Destroy(planet.gameObject);
        }
        planetsToDie.Clear();
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
            planetsToDie.Add(planet2);
            if ( planet2.transform == ControlPanel.Instance.Target ) { ControlPanel.Instance.SetTarget(planet1.transform); }

            Debug.Log(planet1.PlanetName + " eats " + planet2.PlanetName);
        }
        // 행성2가 행성1에 비해 질량이 몹시 큰 경우
        else if ( planet2.PlanetMass >= planet1.PlanetMass*1.5f )
        {
            planet2.PlanetExtend(planet1.PlanetMass * 0.7f);
            planetsToDie.Add(planet1);
            if ( planet1.transform == ControlPanel.Instance.Target ) { ControlPanel.Instance.SetTarget(planet2.transform); }

            Debug.Log(planet2.PlanetName + " eats " + planet1.PlanetName);
        }
        // 두 행성간 질량이 비슷한 경우: 모두 소멸
        else
        {
            planetsToDie.Add(planet1);
            planetsToDie.Add(planet2);

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
        GameObject planet = Instantiate(planetPrefab, transform);
        planet.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
        planet.GetComponent<Planet>().Setup(_name, _mass);
        planet.transform.position   = Random.insideUnitSphere * range;
        planet.transform.localScale = Vector3.one * _radius * 2;

        planets.Add(planet.GetComponent<Planet>());
    }
}
