using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FramesAnimation : MonoBehaviour
{
    protected enum anim_status
    {
        unstart,
        running,
        pause,
        stop
    }

    protected Image image
    {
        get
        {
            return GetComponent<Image>();
        }
    }
    //当前的序列帧动画
    public List<Sprite> image_list = new List<Sprite>();

    //一串连续播放的序列帧动画组
    public List<List<Sprite>> Sequence = new List<List<Sprite>>();
    //当前播放的序列帧ID（如果ID=-1，则不播放序列帧动画。其他时按照ID来播放动画，播完一个动画自动切到下一个动画，全部播完时停留在最后一个动画）
    protected int SequenceID = -1;

    protected bool loop = true;
    protected anim_status status;

    protected UnityAction onComplete;
    //添加动画完成事件，动画第一次循环结束时执行
    public void OnComplete(UnityAction _event)
    {
        onComplete = _event;
    }
    //动画停止时执行一次。
    protected UnityAction EndEvent;
    public void SetEndEvent(UnityAction _event)
    {
        EndEvent = _event;
    }
    //动画每次循环都执行
    protected UnityAction CircleEvent;
    public void SetCircleEvent(UnityAction _event)
    {
        CircleEvent = _event;
    }

    protected UnityAction Change_event;
    //当前帧索引
    private int currentFrameIndex = 0;
    //动画帧率
    private float Framerate = 20.0f;


    void Update()
    {
        if (status == anim_status.running)
        {
            //按帧率播放
            if (Time.frameCount % (30 / Framerate) == 0)
            {
                currentFrameIndex++;
            }

            //第一次播放结束
            if (currentFrameIndex >= image_list.Count)
            {
                SequenceRule();
                currentFrameIndex = 0;
                CircleEvent?.Invoke();
                onComplete?.Invoke();
                if (onComplete != null)
                {
                    onComplete = null;
                }
                Change_event?.Invoke();
                if (!loop && SequenceID == -1)
                {
                    status = anim_status.stop;
                    //停在最后一帧
                    currentFrameIndex = image_list.Count - 1;
                }
            }
            if (image_list.Count != 0)
            {
                image.sprite = image_list[currentFrameIndex];
            }
            else
            {
                Debug.LogError("动画序列帧为空！");
            }
        }
    }

    public void ClearSequence()
    {
        status = anim_status.stop;
        image_list.Clear();
        Sequence.Clear();
    }

    void SequenceRule()
    {
        if (SequenceID != -1)
        {
            if (SequenceID < Sequence.Count - 1)
            {
                image_list.Clear();
                SequenceID++;
                Sequence[SequenceID].ForEach(i => image_list.Add(i));
            }
            else
            {
                SequenceID = -1;
            }
        }
    }

    public void AnimPlaySequence(List<List<Sprite>> _sequence)
    {
        ClearSequence();
        SequenceID = 0;
        currentFrameIndex = 0;
        foreach (List<Sprite> _list in _sequence)
        {
            Sequence.Add(_list);
        }
        Sequence[0].ForEach(i => image_list.Add(i));
        image.sprite = image_list[0];
        loop = true;
        status = anim_status.running;
    }

    public void AnimPlay(List<Sprite> _list, bool _loop = false, int frame = 0)
    {
        ClearSequence();
        SequenceID = 0;
        currentFrameIndex = 0;
        _list.ForEach(i => image_list.Add(i));
        image.sprite = image_list[0];
        loop = _loop;
        currentFrameIndex = frame;
        status = anim_status.running;
    }

    public void AnimPause()
    {
        status = anim_status.pause;
    }

    public void AnimContinue()
    {
        status = anim_status.running;
    }

    public void AnimReplay()
    {
        currentFrameIndex = 0;
    }

    public void AnimStop()
    {
        status = anim_status.stop;
        currentFrameIndex = 0;
        EndEvent?.Invoke();
    }

    public void ChangeClip(List<Sprite> new_list, bool Immediately = false)
    {
        Change_event = delegate
        {
            image_list.Clear();
            new_list.ForEach(i => image_list.Add(i));
        };
        if (Immediately)
        {
            Change_event?.Invoke();
        }
        else
        {
            Change_event += delegate
            {
                Change_event = null;
            };
        }
    }
}
