using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramwork.FlyChess
{

public class FollowTarget: MonoBehaviour
{
    private Transform player;
    public float Ahead = 1.0f;//当角色向右移动时，摄像机比任务位置领先，当角色向左移动时，摄像机比角色落后
    public Vector3 Targetpos;//摄像机的最终目标
    public float smooth = 1.0f;//摄像机平滑移动的值
    private int sence_idx = 0; // 场景idx


    private Vector3 offset;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // offset = player.position - transform.position ;
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.B) && sence_idx==0) 
        {
            // sence_idx+=2;
        }
        // this.transform.position = player.position - offset;
        Targetpos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        if (player.transform.localScale.x > 0)
        {
            Targetpos = new Vector3(player.transform.position.x + Ahead, player.transform.position.y-sence_idx, transform.position.z);
        }
        if (player.transform.localScale.x < 0)
        {
            Targetpos = new Vector3(player.transform.position.x - Ahead, player.transform.position.y-sence_idx, transform.position.z);
        }
        //让摄像机进行平滑的移动
        transform.position = Vector3.Lerp(transform.position, Targetpos, smooth);
    }

}
}