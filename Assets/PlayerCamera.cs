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
    Vector3 mousePosition;
    Vector2 screen;
    Camera mainCamera;

    Vector3 move = Vector3.zero;
    Vector3 cameraMidPoint = Vector3.zero;
    Vector3 centerWPos = Vector3.zero; // Camera Center World Position
    Vector3 mouseWPos = Vector3.zero; // Mouse World Position
    Vector3 cameraStartPosition = Vector3.zero;

    Vector3 camDirectionTowardsMouse = Vector3.zero;
    Vector3 camDirectionTowardsView = Vector3.zero;

    void Start()
    {
        mainCamera = Camera.main;
        cameraStartPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ScreenInfoPolling();
        InputPolling();
        CalculateCameraDirection();

        CameraMovement();
        
        Debug.Log("Forward: " + Vector3.forward);
    }

    void CameraMovement(){
        
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

    void InputPolling(){
        
        /* Keyboard Movement */
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        /* Mouse Movement */
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");       
        
    }

    void ScreenInfoPolling(){
        screen = new Vector2(Screen.width,Screen.height); // Calculate Everytime Incase Of Resize
        mousePosition = new(Input.mousePosition.x,Input.mousePosition.y,mainCamera.nearClipPlane);

        // Get Camera MidPoint
        cameraMidPoint = new (0.5f,0.5f,mainCamera.nearClipPlane);

        // Get the Camera & Mouse World Space Positions
        centerWPos = mainCamera.ViewportToWorldPoint(cameraMidPoint);
        mouseWPos = mainCamera.ScreenToWorldPoint(mousePosition);
    }

    void CalculateCameraDirection(){
        /* Camera Direction Towards  Mouse*/
        centerWPos.y = transform.position.y;
        mouseWPos.y = transform.position.y;

        camDirectionTowardsMouse = (centerWPos - mouseWPos).normalized;
        camDirectionTowardsMouse.y = 0;
    }

    void HandleMouseCameraMovement(){
        move = camDirectionTowardsMouse * cameraSpeed * Time.deltaTime;
    }

    void HandleKeyboardCameraMovement(){
        move = (Vector3.forward * vertical + Vector3.right * horizontal) * cameraSpeed * Time.deltaTime;
    }

    void HandleZoomCameraMovement(){
        Vector3 zoomDirection = (centerWPos - transform.position).normalized * zoomSpeed * Time.deltaTime;

        float rawScroll = Input.mouseScrollDelta.y;
        accumulatedScroll += rawScroll * zoomSpeed;

        if(Mathf.Abs(rawScroll) <= 0.2f){
            accumulatedScroll = 0f; // reset
        }

        if(Input.GetKey(KeyCode.Equals) || accumulatedScroll < 0){
            move -= zoomDirection;// Zoom In
        }else if (Input.GetKey(KeyCode.Minus) || accumulatedScroll > 0){
            move += zoomDirection; // Zoom Out
        }

        float zoomMin = cameraStartPosition.y - zoomInDistance;
        float zoomMax = cameraStartPosition.y + zoomOutDistance;
        float clampedY = Mathf.Clamp(transform.position.y,zoomMin,zoomMax);

        transform.position = new Vector3(transform.position.x,clampedY,transform.position.z);

        // Debug.Log("Zoom" + zoomDirection);
    }

    void HandleRotateCameraMovement(){
        float rotationMagnitude = mouseX * Time.deltaTime * rotateSpeed;
        // Debug.Log("Rotation Magnitude" + rotationMagnitude);
        if(Input.GetKey(KeyCode.R)){
            transform.RotateAround(centerWPos, Vector3.up, rotationMagnitude);
            CalculateCameraDirection();
        }
    
    }

    
    bool MouseCanMoveCamera(){
        if (mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x > screen.x || mousePosition.y > screen.y) return false;

        return  mousePosition.y <= screenSafeZone.y ||   
                mousePosition.y >= screen.y - screenSafeZone.y || 
                mousePosition.x <= screenSafeZone.x || 
                mousePosition.x >= screen.x - screenSafeZone.x;
    }
}
