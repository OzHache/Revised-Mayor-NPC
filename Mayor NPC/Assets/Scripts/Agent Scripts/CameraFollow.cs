using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject m_player;
    private GameObject m_target;
    [SerializeField] private float lag = 5.0f;
    [SerializeField] private float m_speed = 2.0f;
    Coroutine coroutine;
    private bool m_pauseMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_target = m_player;
        coroutine = StartCoroutine(MoveCamera());
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    internal void SetActive(bool v)
    {
        m_pauseMovement = !v;
    }

    private IEnumerator MoveCamera()
    {
        while (true)
        {
            if (m_pauseMovement)
            {
                yield return null;
            }

            Vector3 newPos = Vector3.Lerp(transform.position, m_target.transform.position, lag);
            newPos.z = transform.position.z;
            transform.position = newPos;
            yield return null;
        }
    }

    public void MoveToTarget(GameObject @object, float time)
    {
        StopCoroutine(coroutine);

        StartCoroutine(MoveToTargetCoroutine(@object, time));
    }
    /// <summary>
    /// Moves to the target, tracks for time and then returns to the player
    /// </summary>
    /// <param name="newTarget">new Target to track</param>
    /// <param name="time">how long to track this new target when I arrive</param>
    /// <returns></returns>
    private IEnumerator MoveToTargetCoroutine(GameObject newTarget, float time)
    {
        GameManager.GetGameManager().PauseAction(true);

        //Get the direction to the target
        Vector2 dest2D = newTarget.transform.position;
        Vector2 start2D = transform.position;
        //keeps z at zero
        Vector3 direction = (dest2D - start2D).normalized;
        float dist = Vector2.Distance(dest2D, start2D);
        //move to the target
        StopCoroutine(coroutine);
        while (dist > 0.5f)
        {
            transform.Translate(direction * Time.deltaTime * m_speed);
            dist = Vector2.Distance(dest2D, transform.position);
            yield return null;
        }
        //change the tracking to this
        m_target = newTarget;
        //pause action and start move camera

        yield return new WaitForSeconds(time);
        //stop move camera and move back
        StopCoroutine(coroutine);
        dest2D = m_player.transform.position;
        start2D = transform.position;
        direction = (dest2D - start2D).normalized;
        dist = Vector2.Distance(dest2D, start2D);

        while (dist > 0.5f)
        {
            transform.Translate(direction * Time.deltaTime * m_speed);
            dist = Vector2.Distance(dest2D, transform.position);
            yield return null;
        }
        GameManager.GetGameManager().PauseAction(false);
        //start move Camera again
        m_target = m_player;
        coroutine = StartCoroutine(MoveCamera());
    }

    internal void ChangeTarget(GameObject gameObject)
    {
        m_target = gameObject;
    }
}
