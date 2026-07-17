using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonkeFPS
{
    [BepInPlugin("com.MonkeFPS.ace", "MonkeFPS Display", "1.0.0")]
    public class FPSDisplayMod : BaseUnityPlugin
    {
        public static FPSDisplayMod Instance;
        public static ManualLogSource Log;

        private bool showFPS = true;
        private float deltaTime = 0f;
        private float fps = 0f;
        private float updateInterval = 0.5f;
        private float timer = 0f;
        private int frameCount = 0;

        private Vector2 fpsPosition = new Vector2(10, 10);
        private bool isDragging = false;
        private Vector2 dragOffset;

        private Color fpsColor = Color.green;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                GameObject.DontDestroyOnLoad(gameObject);
                Log = Logger;
                Log.LogInfo("FPS Display Mod Loaded!");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
            {
                showFPS = !showFPS;
                Log.LogInfo($"FPS Display: {(showFPS ? "Shown" : "Hidden")}");
            }

            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            frameCount++;
            timer += Time.unscaledDeltaTime;

            if (timer >= updateInterval)
            {
                fps = frameCount / timer;
                frameCount = 0;
                timer = 0f;

                if (fps >= 87)
                    fpsColor = Color.green;
                else if (fps >= 50)
                    fpsColor = new Color(1f, 0.7f, 0f); 
                else if (fps >= 30)
                    fpsColor = Color.yellow;
                else
                    fpsColor = Color.red;
            }
        }

        void OnGUI()
        {
            if (!showFPS) return;

            string fpsText = $"FPS: {fps:F1}";
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(fpsText));
            
            Rect fpsRect = new Rect(fpsPosition.x, fpsPosition.y, size.x + 20, size.y + 10);
            
            if (Event.current.type == EventType.MouseDown && fpsRect.Contains(Event.current.mousePosition))
            {
                isDragging = true;
                dragOffset = Event.current.mousePosition - fpsPosition;
            }
            if (Event.current.type == EventType.MouseUp)
            {
                isDragging = false;
            }
            if (isDragging && Event.current.type == EventType.MouseDrag)
            {
                fpsPosition = Event.current.mousePosition - dragOffset;
                fpsPosition.x = Mathf.Clamp(fpsPosition.x, 0, Screen.width - 200);
                fpsPosition.y = Mathf.Clamp(fpsPosition.y, 0, Screen.height - 100);
            }

            GUI.color = Color.black;
            GUI.Label(new Rect(fpsPosition.x + 1, fpsPosition.y + 1, size.x, size.y), fpsText, GetFPSLabelStyle(Color.black));
            
            GUI.color = fpsColor;
            GUI.Label(new Rect(fpsPosition.x, fpsPosition.y, size.x, size.y), fpsText, GetFPSLabelStyle(fpsColor));
            GUI.color = Color.white;
        }

        GUIStyle GetFPSLabelStyle(Color color)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontSize = 28;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;
            style.padding = new RectOffset(5, 5, 5, 5);
            return style;
        }

        void OnDestroy()
        {
            
        }
    }
}
