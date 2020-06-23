using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class FramesTest : MonoBehaviour
{
    public List<Sprite> list1;
    public List<Sprite> list2;
    public List<Sprite> list3;
    public FramesAnimation FA1;

    void Update()
    {
        //单序列帧循环播放
        if (Input.GetKeyDown(KeyCode.A)) 
        {
            FA1.AnimPlay(list1, true);
        }

        //多序列帧按顺序播放(播到最后一段保持循环)
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            FA1.AnimPlaySequence(new List<List<Sprite>> { list1, list2,list3 });
        }

        //暂停
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            FA1.AnimPause();
        }

        //继续
        if (Input.GetKeyDown(KeyCode.C))
        {
            FA1.AnimContinue();
        }
    }
}
