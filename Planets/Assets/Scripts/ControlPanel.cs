using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlPanel : MonoBehaviour, IPointerDownHandler
{
    public static ControlPanel Instance { private set; get; }
    private void Awake() => Instance = this;

    [SerializeField] private Transform target;
    private Transform target0;

    [Header("Camera Control")]
    [SerializeField] private Transform cam;
    [SerializeField] private float distance; // 카메라와 target과의 거리
    private Vector3 prePosition;

    [Header("Planet Info Panel")]
    [SerializeField] private GameObject planetInfoPanel;
    [SerializeField] private Text       planetNameText;
    [SerializeField] private Text       planetInfoText;

    [Header("New Planet Panel")]
    [SerializeField] private GameObject newPlanetPanel;
    [SerializeField] private InputField nameInput;
    [SerializeField] private Scrollbar  radiusScrollbar;
    [SerializeField] private Scrollbar  massScrollbar;
    [SerializeField] private float      minRadius;
    [SerializeField] private float      maxRadius;
    [SerializeField] private float      minMass;
    [SerializeField] private float      maxMass;

    public Transform Target { get => target; }


    private void Start()
    {
        // 임의로 100개 행성 추가
        for ( int i=0; i<100; ++i )
        {
            string newName  = "Planet0" + Random.Range(0, 1000);
            float newRadius = Random.Range(minRadius, maxRadius);
            float newMass   = Mathf.Min(newRadius * 1.2f, maxMass);
            PlanetManager.Instance.NewPlanet(newName, newRadius, newMass);
        }

        UpdatePlanetInfo();
    }

    private void Update()
    {
        Zoom();
        Rotate();
        UpdatePlanetInfo();
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
                    SetTarget(target0);
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
        planetInfoText.text = string.Format("{0:0.0}   radius: {1:.0}   mass: {2:.0}",
            planet.transform.position, planet.transform.localScale.x/2, planet.PlanetMass);
    }

    /// <summary>
    /// 카메라 줌 In/Out
    /// </summary>
    private void Zoom()
    {
        if ( target == null ) { return; }

        distance -= Input.GetAxis("Mouse ScrollWheel") * 1000 * target.localScale.x * Time.deltaTime;
        float minDistance = 60   * target.localScale.x / 100;
        float maxDistance = 1000 * target.localScale.x / 100;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        cam.position = Vector3.Lerp(cam.position, cam.rotation * new Vector3(0, 0, -distance) + target.position, 5*Time.deltaTime);
    }

    /// <summary>
    /// 카메라 회전
    /// </summary>
    private void Rotate()
    {
        if ( target == null ) { return; }

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

    /// <summary>
    /// Set target to control (call outside)
    /// </summary>
    public void SetTarget(Transform _target)
    {
        if ( _target == null )
        {
            List<Planet> planets = new List<Planet>(PlanetManager.Instance.Planets);
            planets.Sort(
                (a, b) => Vector3.Distance(cam.position, a.transform.position).CompareTo(Vector3.Distance(cam.position, b.transform.position))
            );
            _target = planets[0].transform;
        }

        target   = _target;
        target0  = null;
        distance = target.localScale.x;
    }

    /// <summary>
    /// UI 숨기기 버튼
    /// </summary>
    public void HideBtn(RectTransform self)
    {
        float angle = self.rotation.eulerAngles.z;
        if ( angle == 0f )
        {
            self.parent.position += new Vector3(0, -(self.parent.GetComponent<RectTransform>().rect.height + 10), 0);
        }
        else if ( angle == 270f )
        {
            self.parent.position += new Vector3(-(self.parent.GetComponent<RectTransform>().rect.width + 10), 0, 0);
        }
        else if ( angle == 180f )
        {
            self.parent.position += new Vector3(0, (self.parent.GetComponent<RectTransform>().rect.height + 10), 0);
        }
        else if ( angle == 90f )
        {
            self.parent.position += new Vector3((self.parent.GetComponent<RectTransform>().rect.width + 10), 0, 0);
        }
        self.rotation = Quaternion.Euler(0, 0, angle - 180);
    }

    /// <summary>
    /// 새로운 행성 생성 버튼
    /// </summary>
    public void NewPlanetBtn()
    {
        string newName   = nameInput.text != "" ? nameInput.text : "Planet0" + Random.Range(0, 1000);
        float  newRadius = Mathf.Max(minRadius, radiusScrollbar.value * maxRadius);
        float  newMass   = Mathf.Max(minMass, massScrollbar.value * maxMass);
        PlanetManager.Instance.NewPlanet(newName, newRadius, newMass);
    }
}
