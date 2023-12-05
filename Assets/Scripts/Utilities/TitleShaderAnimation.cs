using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleShaderAnimation : MonoBehaviour
{
    private const string shineLocationName = "_ShineLocation";

    [Header("Shine")]
    [SerializeField] private bool shineOn = true;
  //  [ShowIf("shineOn")]
    [Range(0.1f, 3)]
    [SerializeField] private float shineDuration = 2.3f;
  //  [ShowIf("shineOn")]
    [Range(0, 10)]
    [SerializeField] private float shineLoopDelay = 1f;
  //  [ShowIf("shineOn")]
    [SerializeField] private bool pingPong = true;
    [SerializeField] private Image foodmessImage;
    [SerializeField] private Image ultraImage;


    [Header("Bob")]
    [SerializeField] private bool bobOn = true;
  //  [ShowIf("bobOn")]
    [Range(.1f, 5)]
    [SerializeField] private float bobDuration = 1f;
  //  [ShowIf("bobOn")]
    [Range(0, 50)]
    [SerializeField] private float bobRange = 3f;
    [SerializeField] private Ease bobEase = Ease.OutSine;
    [SerializeField] private Ease bobReturnEase = Ease.InSine;

    [SerializeField] private Vector3 bobRotationRange = new Vector3(0f,0f,0.6f);
    [SerializeField] private Ease bobRotationEase = Ease.OutSine;
    [SerializeField] private Ease rotationReturnEase = Ease.InSine;

    [SerializeField] private float rotationDuration = 0.8f;
    [SerializeField] private Transform ultraText;
    [SerializeField] private float ultraRotationDelay = 0.4f;

  //  private BetterOffsetter offsetter = null;

    private void Awake()
    {
        foodmessImage.material = new Material(foodmessImage.material);
        ultraImage.material = new Material(ultraImage.material);
       // offsetter = transform.parent.GetComponent<BetterOffsetter>();

        if (shineOn)
        {
            PlayShineAnimation();
        }
        /*if (bobOn)
        {
            PlayBobAnimation();
        }*/
    }

    private void PlayShineAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.PrependInterval(shineLoopDelay);
        sequence.Append(DOTween.To(() => GetShineLocation(ultraImage), x => SetShineLocation(ultraImage, x), 
            endValue: 0f, duration: 0f));
        sequence.Join(DOTween.To(() => GetShineLocation(foodmessImage), x => SetShineLocation(foodmessImage, x),
            endValue: 0f, duration: 0f));
        sequence.Append(DOTween.To(() => GetShineLocation(ultraImage), x => SetShineLocation(ultraImage, x), 
            endValue: 1f, duration: shineDuration));
        sequence.Join(DOTween.To(() => GetShineLocation(foodmessImage), x => SetShineLocation(foodmessImage, x),
            endValue: 1f, duration: shineDuration));


        if (pingPong)
        {
            sequence.SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            sequence.SetLoops(-1);
        }
    }

    /*private void PlayBobAnimation()
    {
        //transform.parent.DORotate(bobRotationRange, rotationDuration).SetEase(bobRotationEase).SetLoops(-1, LoopType.Yoyo); //colocar dentro da sequencia
        Sequence sequence = DOTween.Sequence();
        //sequence.SetEase(Ease.Linear);
        sequence.Append(offsetter.DOAnchoredPositionY(bobRange, bobDuration).SetEase(bobEase));
        sequence.Append(offsetter.DOAnchoredPositionY(0, bobDuration).SetEase(bobReturnEase));
        sequence.Append(offsetter.DOAnchoredPositionY(-bobRange, bobDuration).SetEase(bobEase));
        sequence.Append(offsetter.DOAnchoredPositionY(0, bobDuration).SetEase(bobReturnEase));
        sequence.SetLoops(-1);


        Sequence rotationSequence = DOTween.Sequence();
        rotationSequence.Append(transform.DORotate(bobRotationRange, rotationDuration).SetEase(bobRotationEase));
        rotationSequence.Append(transform.DORotate(Vector3.zero, rotationDuration).SetEase(rotationReturnEase));
        rotationSequence.Append(transform.DORotate(-bobRotationRange, rotationDuration).SetEase(bobRotationEase));
        rotationSequence.Append(transform.DORotate(Vector3.zero, rotationDuration).SetEase(rotationReturnEase));
        rotationSequence.SetLoops(-1);

        Invoke("PlayUltraRotation", ultraRotationDelay);

    }
    */

    private void PlayUltraRotation()
    {
        Sequence ultraRotationSequence = DOTween.Sequence();
        ultraRotationSequence.Append(ultraText.DORotate(bobRotationRange, rotationDuration).SetEase(bobRotationEase));
        ultraRotationSequence.Append(ultraText.DORotate(Vector3.zero, rotationDuration).SetEase(rotationReturnEase));
        ultraRotationSequence.Append(ultraText.DORotate(-bobRotationRange, rotationDuration).SetEase(bobRotationEase));
        ultraRotationSequence.Append(ultraText.DORotate(Vector3.zero, rotationDuration).SetEase(rotationReturnEase));
        ultraRotationSequence.SetLoops(-1);
    }

    private float GetShineLocation(Image img)
    {
        return img.material.GetFloat(shineLocationName);
    }

    private void SetShineLocation(Image img, float value)
    {
        img.material.SetFloat(shineLocationName, value);
    }
}