using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float cameraSpeed = 100f;
    public Vector2 screenSafeZone = new(7f,7f);
    public float zoomSpeed = 500f;
    public float rotateSpeed = 500f;
    public float zoomOutDistance = 1000f;
    public float zoomInDistance = 500f;
        
    [HideInInspector]
    public float horizontal = 0f; 
    [HideInInspector]
    public float vertical = 0f;
    [HideInInspector]
    public float mouseX = 0f;
    [HideInInspector]
    public float mouseY = 0f;
    [HideInInspector]
    public float accumulatedScroll = 0f; 

    [HideInInspector]
    public Vector2 screen;
    [HideInInspector]
    public Camera mainCamera;

    [HideInInspector]
    public Vector3 move = Vector3.zero;
    [HideInInspector]
    public Vector3 cameraInitialPosition = Vector3.zero; 
    [HideInInspector]
    public bool isRotating = false;
    void Start()
    {
        mainCamera = Camera.main;
        cameraInitialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GetUserInput();

        screen = GetScreenDimension();

        HandleCameraMovement();
        
    }

    void HandleCameraMovement(){
        
        // The Vector that Determines which Direction the Camera Will Go.
        move = Vector3.zero;
        
        if(!isRotating){
            if(MouseCanMoveCamera()){
                HandleMouseCameraMovement();
            }
            else{
                HandleKeyboardCameraMovement();
            }
        }

        HandleZoomCameraMovement();
        HandleRotateCameraMovement();

        transform.position -= move;
    }

    void GetUserInput(){        
        /* Keyboard Input */
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        /* Mouse Input */
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");       
    }

    public Vector2 GetScreenDimension(){
        return new (Screen.width,Screen.height); // Calculate Everytime Incase Of Resize
    }

    public Vector3 GetCameraWorldPosition(){
        // Get Camera MidPoint
        Vector3 cameraMidPoint = GetCameraViewPortMidPoint();
        
        // Get the Camera World Space Positions
        return mainCamera.ViewportToWorldPoint(cameraMidPoint);
    }

    public Vector3 GetCameraViewPortMidPoint(){
        return new (0.5f,0.5f,mainCamera.nearClipPlane);
    }

    public Vector3 GetMouseScreenPosition(){
        return new(Input.mousePosition.x,Input.mousePosition.y,mainCamera.nearClipPlane);
    }

    public Vector3 GetMouseWorldPosition(){
        Vector3 mouseScreenPosition = new(Input.mousePosition.x,Input.mousePosition.y,mainCamera.nearClipPlane);
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    public Vector3 GetCameraDirectionToMouse(){
        Vector3 cameraCenterWorldPosition = GetCameraWorldPosition();
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        cameraCenterWorldPosition.y = transform.position.y; // Clamp the Y component so that it doesn't move when mouse moves
        mouseWorldPosition.y = transform.position.y; // Clamp the Y component so that it doesn't move when mouse moves
        /* Camera Direction Towards  Mouse */
        Vector3 dir = (cameraCenterWorldPosition - mouseWorldPosition).normalized;
        dir.y = 0;
        return dir;
    }

    public Vector3 GetCameraDirectionTowardsCenter(){
        Vector3 camWPos = GetCameraWorldPosition();
        camWPos.y = transform.position.y;
        Vector3 dir = (transform.position-GetCameraWorldPosition()).normalized;
        dir.y = 0;
        return dir;
    }

    public Vector3? GetMouseToGroundPosition(LayerMask layerMask = default)
    {
        if(layerMask == default){
            layerMask = LayerMask.GetMask("Ground");
        }
        
        Ray ray = mainCamera.ScreenPointToRay(GetMouseScreenPosition());

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f, layerMask))
        {
            Debug.Log("Ray has hit : " + hit.transform.position);
            return hit.point;
        }
        else
        {
            Debug.Log("Ray didn't hit any ground!");
        }

        return null;
    }

    public Vector3? GetCameraToGroundPosition(LayerMask layerMask = default){
        if(layerMask == default){
            layerMask = LayerMask.GetMask("Ground");
        }
        Ray ray = mainCamera.ViewportPointToRay(GetCameraViewPortMidPoint());
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 10000f, layerMask)){
            return hit.point;
        }else{
            Debug.Log("Ray didn't hit any ground!");
        }
        return null;
    }

    void HandleMouseCameraMovement(){
        move = GetCameraDirectionToMouse() * cameraSpeed * Time.deltaTime;
    }

    void HandleKeyboardCameraMovement(){
        Vector3 forwardVector = GetCameraDirectionTowardsCenter();
        Vector3 rightVector = Vector3.Cross(Vector3.up,forwardVector);
        move = (forwardVector * vertical + rightVector * horizontal) * cameraSpeed * Time.deltaTime;
    }

    void HandleZoomCameraMovement(){
        
        Vector3 cameraCenterWorldPosition = GetCameraWorldPosition();
        Vector3 zoomDirection = (cameraCenterWorldPosition - transform.position).normalized * zoomSpeed * Time.deltaTime;

        float rawScroll = Input.mouseScrollDelta.y;
        accumulatedScroll += rawScroll * zoomSpeed; // to add acceleration

        if(Mathf.Abs(rawScroll) <= 0.2f){
            accumulatedScroll = 0f; // reset
        }

        if(Input.GetKey(KeyCode.Equals) || accumulatedScroll < 0){
            move -= zoomDirection;// Zoom In
        }else if (Input.GetKey(KeyCode.Minus) || accumulatedScroll > 0){
            move += zoomDirection; // Zoom Out
        }

        float zoomMin = cameraInitialPosition.y - zoomInDistance;
        float zoomMax = cameraInitialPosition.y + zoomOutDistance;
        float clampedY = Mathf.Clamp(transform.position.y,zoomMin,zoomMax);

        transform.position = new Vector3(transform.position.x,clampedY,transform.position.z);
    }

    void HandleRotateCameraMovement(){
        float rotationMagnitude = mouseX * Time.deltaTime * rotateSpeed;
        Vector3? rotationPoint = GetCameraToGroundPosition();
        if(Input.GetKey(KeyCode.R) && rotationPoint != null){
            isRotating = true;
            transform.RotateAround((Vector3)rotationPoint, Vector3.up, rotationMagnitude);
        }else{
            isRotating = false;
        }
    }
    
    public bool MouseCanMoveCamera(){
        Vector3 mouseSPos = GetMouseScreenPosition();
        if (mouseSPos.x < 0 || mouseSPos.y < 0 || mouseSPos.x > screen.x || mouseSPos.y > screen.y) return false;

        return  mouseSPos.y <= screenSafeZone.y ||   
                mouseSPos.y >= screen.y - screenSafeZone.y || 
                mouseSPos.x <= screenSafeZone.x || 
                mouseSPos.x >= screen.x - screenSafeZone.x;
    }
}
