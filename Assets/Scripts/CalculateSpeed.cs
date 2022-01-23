using UnityEngine;
using TMPro;

public class CalculateSpeed : MonoBehaviour
{
    [Header("Rigidbody")]
    [SerializeField] Rigidbody player;

    [Header("UI")]
    [SerializeField] TMP_Text speedText;

    [Header("DebugVariables")]
    public float speed = 0f;

    private void Update()
    {
        speed = player.velocity.magnitude * (3.6f / 1.609f);
        speedText.text = "MPH: " + speed.ToString();
    }
}