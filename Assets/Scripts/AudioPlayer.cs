using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource grapplingSound;

    [Header("Script References")]
    public GrapplingGun grapplingGun;

    bool randomDone = false;

    private void Update()
    {
        if (grapplingGun.IsGrappling())
        {
            if (!randomDone)
            {
                grapplingSound.pitch = Random.Range(0.9f, 1.1f);
                randomDone = true;
            }
            
            grapplingSound.enabled = true;
        }
        else
        {
            grapplingSound.enabled = false;
            randomDone = false;
        }
    }
}
