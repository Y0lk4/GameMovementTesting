
using UnityEngine;
using TMPro;

public class SpeedMeter : MonoBehaviour
{

    private Rigidbody rb;

    [Header("UI Reference")]
    public TMP_Text speedText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (speedText == null) return;

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float currentSpeed = flatVelocity.magnitude;
        speedText.text = "Speed: " + currentSpeed.ToString("F1");
    }
}
