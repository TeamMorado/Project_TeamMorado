using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDestory : MonoBehaviour
{
    public TestScript testScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag != "Obstacle")
        {
            if (testScript != null)
            {
                testScript.ListBallUse.Remove(collision.transform.GetComponent<BallScript>());
                Destroy(collision.gameObject);
            }
        }
    }
}
