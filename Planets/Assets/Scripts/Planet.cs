using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Header("Planet Info")]
    [SerializeField] private string planetName;
    [SerializeField] private float  planetMass;
    private Vector3 rotate;
    private float   rotateSpeed;

    public string PlanetName { get => planetName; }
    public float  PlanetMass { get => planetMass; }


    private void Awake()
    {
        rotate      = Random.insideUnitSphere;
        rotateSpeed = 1000 / transform.localScale.x;

        name = planetName;
    }

    private void Update()
    {
        Rotate();
    }

    /// <summary>
    /// �༺ ����
    /// </summary>
    private void Rotate() => transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles + rotate), rotateSpeed*Time.deltaTime);

    /// <summary>
    /// ���ο� �༺ ������ �ʱ� ���� �޼ҵ�
    /// </summary>
    public void Setup(string _planetName, float _planetMass)
    {
        planetName = _planetName;
        planetMass = _planetMass;
        name       = planetName;
    }

    /// <summary>
    /// �༺�� ���� (ũ��/����/ȸ���� ����)
    /// </summary>
    public void PlanetExtend(float mass)
    {
        transform.localScale *= Mathf.Sqrt((planetMass+mass) / planetMass);
        for ( int i=0; i<transform.childCount; ++i )
        {
            transform.GetChild(i).localScale *= Mathf.Sqrt((planetMass + mass) / planetMass);
        }
        planetMass += mass;

        rotate = Random.insideUnitSphere;
        rotateSpeed = 1000 / transform.localScale.x;
    }
}
