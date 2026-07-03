using UnityEngine;
using UnityEngine.InputSystem;
namespace DungeonPairs.UI
{
    public class CustomCursor : MonoBehaviour
    {
        [SerializeField] private Texture2D normalCursor;
        [SerializeField] private Texture2D pressedCursor;
        [SerializeField] private Vector2 hotspot;

        private void Start()
        {
            SetCursor(normalCursor);
        }

        private void Update()
        {
            if (Mouse.current == null)
            {
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                SetCursor(pressedCursor);
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                SetCursor(normalCursor);
            }
        }

        private void SetCursor(Texture2D texture)
        {
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        }
    }
}