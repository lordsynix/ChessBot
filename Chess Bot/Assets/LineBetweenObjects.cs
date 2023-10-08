using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineBetweenObjects : MonoBehaviour
{
    public RectTransform rectTransform1; // Das erste RectTransform
    public RectTransform rectTransform2; // Das zweite RectTransform

    private LineRenderer lineRenderer;

    private void Start()
    {
        // Erstelle einen LineRenderer und füge ihn zu diesem GameObject hinzu
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Setze die Breite der Linie (du kannst dies anpassen)
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // Setze die Farbe der Linie auf Schwarz
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;

        SetLine();
    }

    private void SetLine()
    {
        if (rectTransform1 != null && rectTransform2 != null && lineRenderer != null)
        {
            // Setze die Anfangs- und Endpunkte der Linie basierend auf den RectTransform-Positionen
            Vector3 startPos = rectTransform1.position;
            Vector3 endPos = rectTransform2.position;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
}
