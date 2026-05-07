using UnityEngine;
using UnityEngine.UI;

public class YouTried : MonoBehaviour
{
    public Image image;
    public Image image2;
    public float fadeDuration = 10f;
    private float timer = 10f;
    void Start()
    {
        
    }

    void Update()
    {
        if (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = timer / fadeDuration;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            image2.color = new Color(image2.color.r, image2.color.g, image2.color.b, alpha);
        }
    }

    public void DotheThing()
    {
        timer = 0f;
        Time.timeScale = 0f;
    }
}
