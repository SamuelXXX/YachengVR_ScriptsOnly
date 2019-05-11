using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAlphaDataHolder : MonoBehaviour
{
    [System.Serializable]
    public class ImageSource
    {
        public Image image;
        public Color originalColor;

        public ImageSource(Image image)
        {
            if (image == null)
                return;
            this.image = image;
            originalColor = image.color;
        }


        public void LerpAlpha(float targetAlpha, float t)
        {
            if (image == null)
            {
                return;
            }
            Color targetColor = image.color;
            targetColor.a = targetAlpha * originalColor.a;
            image.color = Color.Lerp(image.color, targetColor, t);
        }
    }

    [System.Serializable]
    public class TextSource
    {
        public Text text;
        public Color originalColor;

        public TextSource(Text text)
        {
            if (text == null)
                return;
            this.text = text;
            originalColor = text.color;
        }

        public void LerpAlpha(float targetAlpha, float t)
        {
            if (text == null)
            {
                return;
            }
            Color targetColor = text.color;
            targetColor.a = targetAlpha * originalColor.a;
            text.color = Color.Lerp(text.color, targetColor, t);
        }
    }

    protected List<ImageSource> allImage = new List<ImageSource>();
    protected List<TextSource> allText = new List<TextSource>();

    public void BuildTarget()
    {
        if (built)
            return;
        allImage.Clear();
        allText.Clear();
        Text[] txtArray = GetComponentsInChildren<Text>();
        Image[] imageArray = GetComponentsInChildren<Image>();

        foreach (var t in txtArray)
        {
            allText.Add(new TextSource(t));
        }

        foreach (var i in imageArray)
        {
            allImage.Add(new ImageSource(i));
        }
        built = true;
    }

    bool built = false;
    public void LerpAlpha(float a, float t)
    {
        foreach (var i in allImage)
        {
            i.LerpAlpha(a, t);
        }

        foreach (var x in allText)
        {
            x.LerpAlpha(a, t);
        }
    }
}
