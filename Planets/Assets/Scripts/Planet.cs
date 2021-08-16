using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Header("Planet Info")]
    [SerializeField] private string planetName;
    private Vector3 rotate;
    private float   rotateSpeed;

    public string PlanetName { get => planetName; }

    private void Awake()
    {
        rotate      = Random.insideUnitSphere;
        rotateSpeed = 1000 / transform.localScale.x;
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles + rotate), rotateSpeed*Time.deltaTime);
    }
}
