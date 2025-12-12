using UnityEngine;

[CreateAssetMenu(fileName = "ActivationAltarSettings", menuName = "Settings/ActivationAltarSettings")]
public class ActivationAltarSettings : ScriptableObject
{

    public float activationDuration = 10f;
    [Header("Screen Shake")]
    public AnimationCurve shakeCurve = new AnimationCurve(new Keyframe(0, 1f), new Keyframe(0.7f, 1), new Keyframe(1f, 0f));
    public float shakeIntensityMultiplier;


    [Header("Altar Activation")]
    public AnimationCurve altarActivationCurve = new AnimationCurve(new Keyframe(0, 1f), new Keyframe(0.7f, 1), new Keyframe(1f, 0f));

    [Header("Tutorial")]
    [Tooltip("Shake curve when player collect map first time")]
    public AnimationCurve tutorialCurve = new AnimationCurve(new Keyframe(0, 1f), new Keyframe(0.7f, 1), new Keyframe(1f, 0f));
    public int tutorialShakeDuration;
}