using UnityEngine;

public class Eraser : MonoBehaviour
{
    [SerializeField] private float tick = 0.05f;

    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f)
            return;

        timer = tick;
        ScratchManager.Scratch(transform.position);
    }
}
