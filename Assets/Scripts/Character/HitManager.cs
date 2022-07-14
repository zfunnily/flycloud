using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// </summary>
namespace PlayerCharacter
{
public class HitManager : MonoBehaviour
{
    /// <summary>
    /// </summary>
    private bool attackTrigger = false;

    /// <summary>
    /// </summary>
    private bool groundTrigger = false;

    /// <summary>
    /// </summary>
    private bool skyTrigger = false;

    /// <summary>
    /// </summary>
    public bool AttackTrigger { set { attackTrigger = value; CheckHit(); } }
    /// <summary>
    /// </summary>
    public bool GroundTrigger { set { groundTrigger = value; CheckHit(); } }
    /// <summary>
    /// </summary>
    public bool SkyTrigger { set { skyTrigger = value; CheckHit(); } }

    private Text m_text;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// </summary>
    public void CheckHit()
    {
        if (attackTrigger && groundTrigger && skyTrigger)
        {
        }
        else
        {
            m_text.text = "";
        }
    }
}
}