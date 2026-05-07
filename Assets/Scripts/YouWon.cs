using UnityEngine;
using UnityEngine.UI;

public class YouWon : MonoBehaviour
{
    public Image image;
    public Image healthBar;
    public HealthSystem playehealth;
    public float fadeDuration = 5f;
    private float timer = 50f;
    private bool backova = false;
    void Start()
    {

    }

    void Update()
    {
        if (timer < fadeDuration && !backova)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            if (timer >= fadeDuration) { timer = fadeDuration; backova = true; }
            return;
        }
        
        if (timer > 0 && backova)
        {
            timer -= Time.deltaTime;
            float alpha = timer / fadeDuration;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }

    public void DotheThing()
    {
        playehealth.Heal(10000f);
        healthBar.enabled = false;
        timer = 0f;
    }
}
