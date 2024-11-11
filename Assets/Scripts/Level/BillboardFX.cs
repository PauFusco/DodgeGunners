using UnityEngine;

public class BillboardFX : MonoBehaviour
{
    private void Update()
    { transform.forward = Camera.main.transform.forward; }
}