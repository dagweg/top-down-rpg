using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 100f;
    [SerializeField] Vector2 screenSafeZone = new(7f,7f);
    [SerializeField] float zoomSpeed = 500f;
    [SerializeField] float rotateSpeed = 500f;
    [SerializeField] float zoomOutDistance = 1000f;
    [SerializeField] float zoomInDistance = 500f;

    float horizontal = 0f; 
    float vertical = 0f;
    float mouseX = 0f;
    float mouseY = 0f;
    float accumulatedScroll = 0f; 

    Vector2 screen;
    Camera mainCamera;

    Vector3 move = Vector3.zero;
    // Vector3 cameraMidPoint = Vector3.zero;
    Vector3 cameraInitialPosition = Vector3.zero; 
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
        
        Debug.Log("Forward: " + Vector3.forward);
    }

    void HandleCameraMovement(){
        
        // The Vector that Determines which Direction the Camera Will Go.
        move = Vector3.zero;
        
        if(MouseCanMoveCamera()){
            HandleMouseCameraMovement();
        }
        else{
            HandleKeyboardCameraMovement();
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

    Vector2 GetScreenDimension(){
        return new (Screen.width,Screen.height); // Calculate Everytime Incase Of Resize
    }

    Vector3 GetCameraWorldPosition(){
        // Get Camera MidPoint
        Vector3 cameraMidPoint = GetCameraMidPoint();
        
        // Get the Camera World Space Positions
        return mainCamera.ViewportToWorldPoint(cameraMidPoint);
    }

    Vector3 GetCameraMidPoint(){
        return new (0.5f,0.5f,mainCamera.nearClipPlane);
    }

    Vector3 GetMouseScreenPosition(){
        return new(Input.mousePosition.x,Input.mousePosition.y,mainCamera.nearClipPlane);
    }

    Vector3 GetMouseWorldPosition(){
        Vector3 mouseScreenPosition = new(Input.mousePosition.x,Input.mousePosition.y,mainCamera.nearClipPlane);
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    Vector3 GetCameraDirectionToMouse(){
        Vector3 cameraCenterWorldPosition = GetCameraWorldPosition();
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        cameraCenterWorldPosition.y = transform.position.y; // Clamp the Y component so that it doesn't move when mouse moves
        mouseWorldPosition.y = transform.position.y; // Clamp the Y component so that it doesn't move when mouse moves
        /* Camera Direction Towards  Mouse */
        Vector3 dir = (cameraCenterWorldPosition - mouseWorldPosition).normalized;
        dir.y = 0;
        return dir;
    }

    Vector3 GetCameraDirectionTowardsCenter(){
        Vector3 camWPos = GetCameraWorldPosition();
        camWPos.y = transform.position.y;
        Vector3 dir = (transform.position-GetCameraWorldPosition()).normalized;
        dir.y = 0;
        return dir;
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
        Vector3 cameraWorldPosition = GetCameraWorldPosition();
        if(Input.GetKey(KeyCode.R)){
            transform.RotateAround(cameraWorldPosition, Vector3.up, rotationMagnitude);
        }
    }
    
    bool MouseCanMoveCamera(){
        Vector3 mouseSPos = GetMouseScreenPosition();
        if (mouseSPos.x < 0 || mouseSPos.y < 0 || mouseSPos.x > screen.x || mouseSPos.y > screen.y) return false;

        return  mouseSPos.y <= screenSafeZone.y ||   
                mouseSPos.y >= screen.y - screenSafeZone.y || 
                mouseSPos.x <= screenSafeZone.x || 
                mouseSPos.x >= screen.x - screenSafeZone.x;
    }
}
