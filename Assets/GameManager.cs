using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotspot = Vector2.zero;

    public int scale = 1;
    void Start()
    {
        if(cursorTexture)
            Cursor.SetCursor(cursorTexture,hotspot,CursorMode.Auto);
    }

    void Update(){
        int maxCurDim = Mathf.Max(cursorTexture.width,cursorTexture.height);
        cursorTexture.width /= maxCurDim * scale;
        cursorTexture.height /= maxCurDim * scale;
    }
}
