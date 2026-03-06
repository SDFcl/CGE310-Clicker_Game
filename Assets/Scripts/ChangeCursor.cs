using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    public Texture2D cursorTexture;

    void Start()
    {
        Vector2 hotspot = new Vector2(32,32);
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
}