using UnityEngine;
using UnityEngine.UI;

public class LineTypeUI : MonoBehaviour
{
    [SerializeField] private StrokeController stroke;

    [SerializeField] private Image normalImage;
    [SerializeField] private Image weightImage;
    [SerializeField] private Image springImage; // 将来用

    [SerializeField] private float selectAlpha = 1f;
    [SerializeField] private float unselectAlpha = 0.3f;

    void Update()
    {
        SetAlpha(normalImage, stroke.type == StrokeController.LineType.Normal);
        SetAlpha(weightImage, stroke.type == StrokeController.LineType.Weight);
        SetAlpha(springImage, stroke.type == StrokeController.LineType.Spring);
    }

    void SetAlpha(Image img, bool selected)
    {
        if (img == null) return;

        Color c = img.color;
        c.a = selected ? selectAlpha : unselectAlpha;
        img.color = c;
    }
}