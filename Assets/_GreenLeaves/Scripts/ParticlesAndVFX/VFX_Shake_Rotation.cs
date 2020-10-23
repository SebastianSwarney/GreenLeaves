using System.Collections;
using UnityEngine;


/// <summary>
/// <para>This script is used to shake the object on it's origin point.
/// In order for this to work correctly, the object's model should have it's pivot point be
/// the origin point.</para>
/// Note: This doesn't create that shaky effect used for the trees. This simply shakes the 
/// object rapidly and randomly between some set points
/// </summary>
public class VFX_Shake_Rotation : MonoBehaviour
{
    public Vector3 m_maxShakeVector;
    public AnimationCurve m_lerpCurve;

    public float m_animTime;

    /// <summary>
    /// Called to shake the object. Usually called through public events
    /// </summary>
    public void StartShake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeAnimation());
    }


    private IEnumerator ShakeAnimation()
    {
        float timer = 0;
        while (timer < m_animTime)
        {
            timer += Time.deltaTime;
            float percent = timer / m_animTime; //Mathf.PerlinNoise(Time.time, Time.time);
            //percent -= m_lerpCurve.Evaluate(timer / m_animTime);

            //transform.localEulerAngles = new Vector3(m_maxShakeVector.x * percent, m_maxShakeVector.y * percent, m_maxShakeVector.z * percent);
            transform.localEulerAngles = new Vector3(Random.Range(-m_maxShakeVector.x, m_maxShakeVector.x), Random.Range(-m_maxShakeVector.y, m_maxShakeVector.y), Random.Range(-m_maxShakeVector.z, m_maxShakeVector.z)) * (1 - m_lerpCurve.Evaluate(timer/m_animTime));
            yield return null;
        }
        transform.localEulerAngles = Vector3.zero;
    }
}
