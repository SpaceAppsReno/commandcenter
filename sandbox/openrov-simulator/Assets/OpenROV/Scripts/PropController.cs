using UnityEngine;
using System.Collections;

public class PropController : MonoBehaviour
{
    public AnimationClip ForwardAnimation;
    public AnimationClip ReverseAnimation;

    public float MaxSpeed = 50.0f;
    public void MoveForward(int speed)
    {
        animation[ForwardAnimation.name].speed = Mathf.Clamp(speed, 0, MaxSpeed);
        animation.Play(ForwardAnimation.name);
    }

    public void MoveReverse(int speed)
    {
        animation[ReverseAnimation.name].speed = Mathf.Clamp(speed, 0, MaxSpeed);
        animation.Play(ReverseAnimation.name);
    }

    public void Stop()
    {
        animation.Stop();
    }
}
