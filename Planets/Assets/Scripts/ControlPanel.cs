using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlPanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform target;
    private Transform target0;

    [Header("Camera Control")]
    [SerializeField] private Transform cam;
    [SerializeField] private float distance;            // 카메라와 target의 거리
    private Vector3 prePosition;

    [Header("Planet Info Panel")]
    [SerializeField] private GameObject planetInfoPanel;
    [SerializeField] private Text       planetNameText;
    [SerializeField] private Text       planetInfoText;

    private void Start()
    {
        UpdatePlanetInfo();
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        Zoom();
        Rotate();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(eventData.position);
        Physics.Raycast(ray, out RaycastHit hit);
        if ( hit.transform != null )
        {
            if ( hit.transform.CompareTag("Planet") )
            {
                if ( (target != hit.transform) && (target0 == hit.transform) )
                {
                    target   = target0;
                    target0  = null;
                    distance = target.localScale.x;

                    UpdatePlanetInfo();
                }
                else
                {
                    target0 = hit.transform;
                }
            }
            else
            {
                target0 = null;
            }
        }
    }

    /// <summary>
    /// 행성 정보 패널 업데이트
    /// </summary>
    private void UpdatePlanetInfo()
    {
        if ( target == null ) { return; }

        Planet planet       = target.GetComponent<Planet>();
        planetNameText.text = planet.PlanetName;
        planetInfoText.text = string.Format("{0}   radius: {1}", planet.transform.position, planet.transform.localScale.x);
    }

    private void Zoom()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel") * 1000 * target.localScale.x * Time.deltaTime;
        float minDistance = 60   * target.localScale.x / 100;
        float maxDistance = 1000 * target.localScale.x / 100;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        cam.position = Vector3.Lerp(cam.position, cam.rotation * new Vector3(0, 0, -distance) + target.position, 5*Time.deltaTime);
    }

    /// <summary>
    /// https://emmaprats.com/p/how-to-rotate-the-camera-around-an-object-in-unity3d/
    /// </summary>
    private void Rotate()
    {
        if ( Input.GetMouseButtonDown(0) )
        {
            prePosition = cam.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
        }
        else if ( Input.GetMouseButton(0) )
        {
            Vector3 curPosition = cam.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction   = prePosition - curPosition;

            float rotationAroundYAxis = -direction.x * 180;
            float rotationAroundXAxis = direction.y * 180;

            cam.transform.position = target.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <- This is what makes it work!

            cam.transform.Translate(new Vector3(0, 0, -distance));

            prePosition = curPosition;
        }
    }
}
