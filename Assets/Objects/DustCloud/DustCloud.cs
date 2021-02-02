using UnityEngine;

public class DustCloud : MonoBehaviour
{ 
    private void OnAnimationEnded()
    {
        Destroy(gameObject);
    }
}
