using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheGame
{
    public class FPSDisplay : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        int m_FpsAccumulator = 0;
        float m_FpsNextPeriod = 0;
        int m_CurrentFps;
        const string display = "{0} FPS";


        void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        }


        void Update()
        {
            // measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int) (m_FpsAccumulator/fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;
            }
        }
        
        void OnGUI()
        {
            var cam = Camera.main;
            if (cam == false) return;
            
            const float WIDTH = 1000f;
            const float HEIGHT = 20f;
            
            var transformPos = transform.position;
            transformPos.z = Mathf.Abs(transformPos.z - cam.transform.position.z);
            var labelOffset = Vector2.up * HEIGHT;

            var screenPoint = cam.WorldToScreenPoint(transformPos);
            var rect = new Rect(screenPoint.x, Screen.height - screenPoint.y, WIDTH, HEIGHT);
            
            DrawLabel(rect, gameObject.name, Color.red);
            rect.position += labelOffset;

            DrawLabel(rect, "Target Frame Rate : " + Application.targetFrameRate, Color.white);
            rect.position += labelOffset;
            
            DrawLabel(rect, string.Format(display, m_CurrentFps), Color.green);
            rect.position += labelOffset;
        }

        void DrawLabel(Rect rect, string text, Color c = default)
        {
            c = c == default ? Color.white : c;
            GUI.color = c;
            GUI.Label(rect, text, GUI.skin.label);
        }
    }
}
