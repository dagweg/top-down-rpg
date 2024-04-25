using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 100f;
    [SerializeField] Vector2 screenBounding = new(7f,7f);
    [SerializeField] float zoomSpeed = 100f;
    [SerializeField] float cameraZoomOutDistance = 10f;

    float horizontal = 0f; 
    float vertical = 0f;
    Vector3 mousePosition;
    Vector2 screen;
    Camera mainCamera;



    Vector3 move = Vector3.zero;
    Vector3 cameraMidPoint = Vector3.zero;
    Vector3 centerWPos = Vector3.zero;
    Vector3 mouseWPos = Vector3.zero;
    Vector3 cameraStartPosition = Vector3.zero;

    void Start()
    {
        mainCamera = Camera.main;
        cameraStartPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ScreenInfoPolling();
        
        MoveCamera();
    }

    void MoveCamera(){
        
        // The Vector that Determines which Direction the Camera Will Go.
        move = Vector3.zero;

        

        if(MouseCanMoveCamera()){
            HandleMouseCameraMovement();
        }
        else{
            HandleKeyboardCameraMovement();
        }

        HandleZoomCameraMovement();

        transform.position -= move;
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

    void HandleMouseCameraMovement(){
        centerWPos.y = transform.position.y;
        mouseWPos.y = transform.position.y;

        Vector3 camDirection = (centerWPos - mouseWPos).normalized;
        camDirection.y = 0;
        
        move = camDirection * cameraSpeed * Time.deltaTime;
    }


    void HandleKeyboardCameraMovement(){
        /* Keyboard Movement */
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        move = new Vector3(horizontal,0,vertical).normalized * cameraSpeed * Time.deltaTime;
    }

    void HandleZoomCameraMovement(){
        if(Input.GetKey(KeyCode.Equals)){
            // Zoom In

        }else if (Input.GetKey(KeyCode.Minus)){
            // Zoom Out
        }
    }

    
    bool MouseCanMoveCamera(){
        //!(mousePosition.y > screenBounding.y || mousePosition.y < 0 || mousePosition.x >screenBounding.x || mousePosition.x < 0) &&
        return  mousePosition.y <= screenBounding.y ||   
                mousePosition.y >= screen.y - screenBounding.y || 
                mousePosition.x <= screenBounding.x || 
                mousePosition.x >= screen.x - screenBounding.x;
    }
}
