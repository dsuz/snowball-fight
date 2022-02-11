using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCharacterController : MonoBehaviour
{
    [SerializeField] GameObject m_ball;
    [SerializeField] Transform _snowballMuzzle;
    Animator m_anim;
    int m_pattern = 0;
    float m_time = 0;
    float m_interval = 3;
    Quaternion m_defaultRotation;
    void Start()
    {
        m_interval = Random.Range(10, 41) / 10f;
        m_defaultRotation = this.transform.rotation;
        m_anim = GetComponent<Animator>();
    }
    void Update()
    {
        m_time += Time.deltaTime;
        if (m_time > m_interval)
        {
            if (m_pattern == 0)
            {
                m_anim.CrossFade("Crouch", 0.1f);
                m_pattern++;
                m_interval = Random.Range(10, 61) / 10f;
            }
            else if (m_pattern == 1)
            {
                m_anim.CrossFade("Pitch", 2);
                m_pattern++;
                m_interval = 1;
            }
            else
            {
                m_anim.CrossFade("Idle", 1);
                m_pattern = 0;
                m_interval = Random.Range(10, 41) / 10f;
            }
            m_time = 0;
        }
    }
    void Crouch()
    {
        this.transform.rotation = m_defaultRotation;
    }
    void PitchSnowBall()
    {
        GameObject g  = Instantiate(m_ball, this._snowballMuzzle.position, this._snowballMuzzle.rotation);
        g.GetComponent<Rigidbody>().AddForce(_snowballMuzzle.forward * 10 + _snowballMuzzle.up * 6, ForceMode.Impulse);
        Destroy(g, 2);
    }
    void ResetStatus()
    {

    }
}
