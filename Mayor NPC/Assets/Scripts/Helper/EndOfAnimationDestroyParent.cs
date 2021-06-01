using System.Collections;
using UnityEngine;

public class EndOfAnimationDestroyParent : MonoBehaviour
{
    private Vector3 m_spawnPosition;

    public void End()
    {
        m_spawnPosition = transform.position;
        Transform parent = transform.parent;
        transform.parent = null;
        Destroy(parent.gameObject);
        GetComponent<Animator>().enabled = false;
        StartCoroutine(ResetPostion());
    }

    IEnumerator ResetPostion()
    {
        yield return null;
        transform.position = m_spawnPosition;
    }
}
