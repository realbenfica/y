﻿// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers
{

    /// <summary>
    /// This script adds a key or button trigger to a Unity UI Selectable.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    [RequireComponent(typeof(UnityEngine.UI.Selectable))]
    public class UIButtonKeyTrigger : MonoBehaviour
    {

        [Tooltip("Trigger the selectable when this key is pressed.")]
        public KeyCode key = KeyCode.None;

        [Tooltip("Trigger the selectable when this input button is pressed.")]
        public string buttonName = string.Empty;

        [Tooltip("Trigger if any key, input button, or mouse button is pressed.")]
        public bool anyKeyOrButton = false;

        [Tooltip("Ignore trigger key/button if UI button is being clicked Event System's Submit input. Prevents unintentional double clicks.")]
        public bool skipIfBeingClickedBySubmit = true;

        [Tooltip("Visually show UI Button in pressed state when triggered.")]
        public bool simulateButtonClick = true;

        [Tooltip("Show pressed state for this duration in seconds.")]
        public float simulateButtonDownDuration = 0.1f;

        private UnityEngine.UI.Selectable m_selectable;

        /// <summary>
        /// Set false to prevent all UIButtonKeyTrigger components from listening for input.
        /// </summary>
        public static bool monitorInput = true;

        private void Awake()
        {
            m_selectable = GetComponent<UnityEngine.UI.Selectable>();
            if (m_selectable == null) enabled = false;
        }

        private void Update()
        {
            if (!monitorInput) return;
            if (InputDeviceManager.IsKeyDown(key) || 
                (!string.IsNullOrEmpty(buttonName) && InputDeviceManager.IsButtonDown(buttonName)) ||
                (anyKeyOrButton && InputDeviceManager.IsAnyKeyDown()))
            {
                if (skipIfBeingClickedBySubmit && IsBeingClickedBySubmit()) return;
                if (simulateButtonClick)
                {
                    StartCoroutine(SimulateButtonClick());
                }
                else
                {
                    ExecuteEvents.Execute(m_selectable.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
                }
            }
        }

        private bool IsBeingClickedBySubmit()
        {
            return EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject != m_selectable.gameObject &&
                InputDeviceManager.instance != null &&
                InputDeviceManager.IsButtonDown(InputDeviceManager.instance.submitButton);
        }

        IEnumerator SimulateButtonClick()
        {
            m_selectable.OnPointerDown(new PointerEventData(EventSystem.current));
            yield return new WaitForSeconds(simulateButtonDownDuration);
            m_selectable.OnDeselect(null);
            ExecuteEvents.Execute(m_selectable.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }

    }

}
