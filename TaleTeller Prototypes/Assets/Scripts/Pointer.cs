using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    public Camera mainCamera;
    public Vector2 screenSize;
    public Transform self;

    public List<RaycastResult> results = new List<RaycastResult>();
    public Vector3 pointerDirection;
    public void Start()
    {
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0, 0, -10);
        pointerDirection = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0).normalized;
    }
}
