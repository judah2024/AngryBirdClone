using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BirdController 
    : MonoBehaviour
    , IPointerDownHandler
    , IDragHandler
    , IPointerUpHandler
{
    [Header("새총 설정")] 
    [SerializeField] private float maxDragDistance = 3f;
    [SerializeField] private float launchPower = 500f;

    [Header("Line Renderer")] 
    [SerializeField] private float lineWidth = 0.1f;
    private LineRenderer stringLine => GameManager.Instance.stringLine;

    [Header("궤적 시각화")] 
    [SerializeField] private int maxObjectCount = 10;
    [SerializeField] private GameObject trajectoryPrefab;
    private List<GameObject> trajectoryList = new List<GameObject>();

    private Vector2 startDragPosition;
    private bool isDragging;
    private bool isLaunched => GameManager.Instance.isLaunched;

    
    Rigidbody2D _rb;
    CircleCollider2D _collider;
    Camera mainCamera;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        mainCamera = Camera.main;

        _rb.isKinematic = true;

        SetupStringLine();
        InitTrajectory();
    }

    void SetupStringLine()
    {
        if (stringLine == null)
            return;

        stringLine.positionCount = 2;
        stringLine.startWidth = lineWidth;
        stringLine.endWidth = lineWidth;
        stringLine.enabled = false;
    }

    void InitTrajectory()
    {
        for (int i = 0; i < maxObjectCount; i++)
        {
            GameObject obj = Instantiate(trajectoryPrefab);
            trajectoryList.Add(obj);
        }
        
        InactiveTrajectory();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLaunched)
            return;

        isDragging = true;
        startDragPosition = transform.position;

        stringLine.enabled = true;

        Debug.Log("Bird grabbed");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || isLaunched)
            return;

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(eventData.position);
        Vector2 slingShotPos = startDragPosition;
        Vector2 direction = mousePos - slingShotPos;

        if (direction.magnitude > maxDragDistance)
        {
            direction = direction.normalized * maxDragDistance;
        }

        transform.position = slingShotPos + direction;
        
        stringLine.SetPosition(0, startDragPosition);
        stringLine.SetPosition(1, transform.position);
        DrawTrajectory(-direction * launchPower * Time.fixedDeltaTime); // rigidBody는 물리시간을 사용하므로 fixedDeltaTime을 적용해야한다.
    }   
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging || isLaunched)
            return;

        isDragging = false;
        GameManager.Instance.isLaunched = true;
        _rb.isKinematic = false;

        Vector2 direction = (startDragPosition - (Vector2)transform.position);
        _rb.AddForce(direction * launchPower);
        
        CameraManager.Instance.ChangeFollowCamera(transform);
        stringLine.enabled = false;
        DestroyTrajectory();
        StartCoroutine(CoNextBird());
    }

    IEnumerator CoNextBird()
    {
        yield return new WaitForSeconds(6f);
        // 다음 새!
        CameraManager.Instance.ChangeMainCamera();
        GameManager.Instance.CheckGameEnd();
        
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void DrawTrajectory(Vector2 power)
    {
        Vector2 s = transform.position;
        Vector2 g = Physics2D.gravity;
        float step = 0.1f;
        for (int i = 0; i < trajectoryList.Count; i++)
        {
            float t = (i + 1) * step;
            Vector2 pos = s + power * t + g * t * t * 0.5f;
            
            trajectoryList[i].transform.position = pos;
            trajectoryList[i].SetActive(true);
        }
    }

    void InactiveTrajectory()
    {
        foreach (var obj in trajectoryList)
        {
            obj.SetActive(false);
        }
    }
    
    void DestroyTrajectory()
    {
        foreach (var obj in trajectoryList)
        {
            Destroy(obj);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isLaunched)
        {
            // 충격 시작
        }
    }
}
