using UnityEngine;

public class Blink : MonoBehaviour
{
    private float blinkDuration = 1f; // Duration of each blink in seconds
    private float blinkCooldown = 1f; // Time between blinks in seconds

    private float blinkTimer = 0f; // Timer to track the blink duration
    private float cooldownTimer = 0f; // Timer to track the cooldown between blinks

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownTimer >= blinkCooldown)
        {
            blinkTimer += Time.deltaTime;

            float blinkProgress = blinkTimer / blinkDuration;
            if (blinkProgress <= 0.5f)
            {
                float closeBlinkProgress = blinkProgress * 2f;
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(0.4f, 0f, closeBlinkProgress), transform.localScale.z);
            }
            else
            {
                float openBlinkProgress = (blinkProgress - 0.5f) * 2f;
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(0f, 0.4f, openBlinkProgress), transform.localScale.z);
            }

            if (blinkTimer >= blinkDuration)
            {
                blinkTimer = 0f;
                cooldownTimer = 0f;
            }
        }
        else
        {
            cooldownTimer += Time.deltaTime;
        }
    }
}
