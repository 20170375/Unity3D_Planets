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
        // �༺�� �߷� �ۿ�
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

        // ����� �༺ ó��
        foreach ( Planet planet in planetsToDie )
        {
            planets.Remove(planet);
            Destroy(planet.gameObject);
        }
        planetsToDie.Clear();
    }

    /// <summary>
    /// �� �༺�� �浹�� ��� ȣ��
    /// </summary>
    private void PlanetCollision(Planet planet1, Planet planet2)
    {
        // �༺1�� �༺2�� ���� ������ ���� ū ���
        if ( planet1.PlanetMass >= planet2.PlanetMass*1.5f )
        {
            planet1.PlanetExtend(planet2.PlanetMass * 0.7f);
            planetsToDie.Add(planet2);
            if ( planet2.transform == ControlPanel.Instance.Target ) { ControlPanel.Instance.SetTarget(planet1.transform); }

            Debug.Log(planet1.PlanetName + " eats " + planet2.PlanetName);
        }
        // �༺2�� �༺1�� ���� ������ ���� ū ���
        else if ( planet2.PlanetMass >= planet1.PlanetMass*1.5f )
        {
            planet2.PlanetExtend(planet1.PlanetMass * 0.7f);
            planetsToDie.Add(planet1);
            if ( planet1.transform == ControlPanel.Instance.Target ) { ControlPanel.Instance.SetTarget(planet2.transform); }

            Debug.Log(planet2.PlanetName + " eats " + planet1.PlanetName);
        }
        // �� �༺�� ������ ����� ���: ��� �Ҹ�
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
    /// ���ο� �༺ ����
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
