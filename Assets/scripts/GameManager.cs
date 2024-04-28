
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotspot = Vector2.zero;

    public Vector2 cursorDimension;

    void Start()
    {
        if(cursorTexture)
            Cursor.SetCursor(cursorTexture,hotspot,CursorMode.Auto);
    }

}
