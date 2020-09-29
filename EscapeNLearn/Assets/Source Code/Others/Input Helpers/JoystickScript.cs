using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRubyShared;

public class JoystickScript : MonoBehaviour
{
    public JoystickInput JoystickInput;
    public Image JoystickImage;

    // Reduces the amount the joystick moves the closer it is to the center. As the joystick moves to it's max extents, the movement amount approaches 1. " +
    // "For example, a power of 1 would be a linear equation, 2 would be squared, 3 cubed, etc.
    private float JoystickPower = 2.0f;

    // Other private vars
    private float MaxExtentPercent = 0.06f;
    private Vector2 startCenter;
    private float maxOffset;
    private PanGestureRecognizer PanGesture;
    private bool MoveJoystickToGestureStartLocation = true;
    private bool _enabled = true;
    private System.Action<Vector2> JoystickExecuted;


    public void SetEnabled(bool flag)
    {
        _enabled = flag;
    }

    private void Start()
    {
        maxOffset = MaxExtentPercent * 1000;
        PanGesture = new PanGestureRecognizer
        {
            PlatformSpecificView = (MoveJoystickToGestureStartLocation ? null : JoystickImage.gameObject),
            ThresholdUnits = 0.0f
        };
        PanGesture.AllowSimultaneousExecutionWithAllGestures();
        PanGesture.Updated += PanGestureUpdated;
        FingersScript.Instance.AddGesture(PanGesture);

        JoystickExecuted = JoystickInput.JoystickExecuted;

#if UNITY_EDITOR

        if (JoystickImage == null || JoystickImage.canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogError("Fingers joystick script requires that JoystickImage be set and that the Canvas be in ScreenSpaceOverlay mode.");
        }
#endif

    }

    private void SetImagePosition(Vector2 pos)
    {
        JoystickImage.rectTransform.anchoredPosition = pos;
    }

    private void ExecuteCallback(Vector2 amount)
    {
        if (JoystickExecuted != null && _enabled)
        {
            JoystickExecuted(amount);
        }
    }

    private void PanGestureUpdated(GestureRecognizer gesture, ICollection<GestureTouch> touches)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            // clamp joystick movement to max values
            float screenFactor = (float)UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
            if (screenFactor >= 1)
            {
                screenFactor = 1;
            }

            maxOffset = MaxExtentPercent * 1000;
            maxOffset = maxOffset * screenFactor;
            Vector2 offset = new Vector2(gesture.FocusX - gesture.StartFocusX, gesture.FocusY - gesture.StartFocusY);

            // check distance from center, clamp to distance
            offset = Vector2.ClampMagnitude(offset, maxOffset);

            // don't bother if no motion
            if (offset == Vector2.zero)
            {
                return;
            }

            // move image
            SetImagePosition(startCenter + offset);

            // callback with movement weight
            if (JoystickPower >= 1.0f)
            {
                // power is reducing offset, apply as is
                offset.x = Mathf.Sign(offset.x) * Mathf.Pow(Mathf.Abs(offset.x) / maxOffset, JoystickPower);
                offset.y = Mathf.Sign(offset.y) * Mathf.Pow(Mathf.Abs(offset.y) / maxOffset, JoystickPower);
            }
            else
            {
                // power is increasing offset, we need to make sure we maintain the aspect ratio of offset
                Vector2 absOffset = new Vector2(Mathf.Abs(offset.x), Mathf.Abs(offset.y));
                float offsetTotal = absOffset.x + absOffset.y;
                float xWeight = absOffset.x / offsetTotal;
                float yWeight = absOffset.y / offsetTotal;
                offset.x = xWeight * Mathf.Sign(offset.x) * Mathf.Pow(absOffset.x / maxOffset, JoystickPower);
                offset.y = yWeight * Mathf.Sign(offset.y) * Mathf.Pow(absOffset.y / maxOffset, JoystickPower);
                offset = Vector2.ClampMagnitude(offset, maxOffset);
            }
            ExecuteCallback(offset);
        }
        else if (gesture.State == GestureRecognizerState.Began)
        {
            if (MoveJoystickToGestureStartLocation)
            {
                JoystickImage.transform.parent.position = new Vector3(gesture.FocusX, gesture.FocusY, JoystickImage.transform.parent.position.z);
                JoystickInput.JoystickMoved();
            }
            startCenter = JoystickImage.rectTransform.anchoredPosition;
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            // return to center
            SetImagePosition(startCenter);

            // final callback
            ExecuteCallback(Vector2.zero);

            JoystickInput.JoystickDisconnected();
        }
    }
}