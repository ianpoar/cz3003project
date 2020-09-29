using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;
using UnityEngine.Events;

[System.Serializable]
public class OnJoystickExecuted : UnityEvent<Vector2> { }
[System.Serializable]
public class OnJoystickDisconnected : UnityEvent<Vector2> { }

public class JoystickInput : MonoBehaviour
{
    // References
    public OnJoystickExecuted OnJSExecuted;
    public OnJoystickDisconnected OnJSDisconnected;
    public GameObject JoystickKnob, JoystickBG;
    public JoystickScript Script;


    // Private var
    private LongPressGestureRecognizer longPressGesture_SpawnJoystick;
    private bool _inputPaused = false;

    public void Start()
    {
        // Create long press gesture to spawn joystick

        // Long Press
        longPressGesture_SpawnJoystick = new LongPressGestureRecognizer();
        longPressGesture_SpawnJoystick.MinimumDurationSeconds = 0;
        longPressGesture_SpawnJoystick.MaximumNumberOfTouchesToTrack = 1;
        longPressGesture_SpawnJoystick.Updated += LongPressGestureCallback_SpawnJoystick;
        FingersScript.Instance.AddGesture(longPressGesture_SpawnJoystick);

        longPressGesture_SpawnJoystick.AllowSimultaneousExecutionWithAllGestures();
    }

    private void Update()
    {
#if UNITY_EDITOR
#else
            // input bug fix on mobile devices
            if (Input.touchCount < 1)
            {
                if (JoystickKnob.activeSelf)
                {
                    longPressGesture_SpawnJoystick.Reset();
                    HideJoystick();
                }
            }
#endif
    }

    // Perform control logic here, using the vector2 value passed on
    public void JoystickExecuted(Vector2 value)
    {
        if (_inputPaused)
        {
            return;
        }

        if (OnJSExecuted != null)
            OnJSExecuted.Invoke(value);
    }

    public void JoystickDisconnected()
    {
        if (OnJSDisconnected != null)
            OnJSDisconnected.Invoke(Vector2.zero);
    }

    // Call this to pause/unpause player control inputs
    public void PauseInput(bool flag)
    {
        if (flag)
        {
            longPressGesture_SpawnJoystick.Reset();
            HideJoystick();
        }

        _inputPaused = flag;
    }

    // Used by FingersJoyStickScript only
    public void HideJoystick()
    {
        JoystickKnob.SetActive(false);
        JoystickBG.SetActive(false);
        Script.SetEnabled(false);
    }

    // Used by FingersJoyStickScript only
    public void JoystickMoved()
    {
        if (_inputPaused)
        {
            return;
        }

        JoystickKnob.SetActive(true);
        JoystickBG.SetActive(true);
        Script.SetEnabled(true);
    }

    // Gesture Functions
    private GestureTouch FirstTouch(ICollection<GestureTouch> touches)
    {
        foreach (GestureTouch t in touches)
        {
            return t;
        }
        return new GestureTouch();
    }

    private void LongPressGestureCallback_SpawnJoystick(GestureRecognizer gesture, ICollection<GestureTouch> touches)
    {
        GestureTouch t = FirstTouch(touches);

        if (gesture.State == GestureRecognizerState.Began) // Begin Touch
        {
            if (_inputPaused)
            {
                return;
            }
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            HideJoystick();
        }
    }
}
