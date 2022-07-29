using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
    public float time = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyEventually());
    }

    private IEnumerator DestroyEventually()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
