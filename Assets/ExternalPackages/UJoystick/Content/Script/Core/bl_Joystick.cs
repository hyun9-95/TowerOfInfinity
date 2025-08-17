using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class bl_Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public int GetTouchID
    {
        get
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].fingerId == lastId)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    private float radio { get { return (Radio * 5 + Mathf.Abs((diff - CenterReference.position.magnitude))); } }
    private float smoothTime { get { return (1 - (SmoothTime)); } }

    public float Horizontal
    {
        get
        {
            if (isFollowStick)
            {
                return (StickRect.position.x - DeathArea.x) / Radio;
            }
            else
            {
                return inputVector.x;
            }
        }
    }

    public float Vertical
    {
        get
        {
            if (isFollowStick)
            {
                return (StickRect.position.y - DeathArea.y) / Radio;
            }
            else
            {
                return inputVector.y;
            }
        }
    }

    public bool IsAvailable
    {
        get { return isInitialized && isUpdate; }
    }

    public bool IsUsing
    {
        get { return Horizontal != 0 || Vertical != 0; }
    }

    [Header("Settings")]
    [SerializeField, Range(1, 15)]
    private float Radio = 5;
    [SerializeField, Range(0.01f, 1)]
    private float SmoothTime = 0.5f;
    [SerializeField, Range(0.5f, 4)]
    private float OnPressScale = 1.5f;
    [SerializeField, Range(0.1f, 5)]
    private float Duration = 1;

    [Header("Reference")]
    [SerializeField]
    private RectTransform StickRect;
    [SerializeField]
    private RectTransform CenterReference;
    [SerializeField]
    private Canvas m_Canvas;
    [SerializeField]
    private Image stickImage;
    [SerializeField]
    private Image backImage;
    [SerializeField]
    private Sprite defaultSprite;
    [SerializeField]
    private Sprite clickSprite;
    [SerializeField]
    private Sprite dragSprite;
    [SerializeField]
    private bool isFollowStick = false;

    private Vector3 DeathArea;
    private Vector3 currentVelocity;
    private bool isFree = false;
    private int lastId = -2;
    private float diff;
    private Vector3 PressScaleVector;
    private Color normalColor = new Color(1, 1, 1, 1);
    private Color pressColor = new Color(1, 1, 1, 1);
    private bool isInitialized = false;
    private bool isUpdate = false;
    private float lastAngleDeg = 0f;
    private const float k_MinDirSqr = 0.0001f;
    private Vector2 inputVector = Vector2.zero;

    public void EnableInput(bool value)
    {
        if (value)
        {
            if (!isInitialized)
                Initialize();
            isUpdate = true;
        }
        else
        {
            isUpdate = false;
            ResetJoystick();
        }
        gameObject.SafeSetActive(value);
    }

    private void Initialize()
    {
        if (StickRect == null)
        {
            Logger.Error("Joystick Initailize Failed!");
            this.enabled = false;
            return;
        }

        DeathArea = CenterReference.position;
        diff = CenterReference.position.magnitude;
        PressScaleVector = new Vector3(OnPressScale, OnPressScale, OnPressScale);

        if (backImage != null)
            backImage.CrossFadeColor(normalColor, 0.1f, true, true);

        if (stickImage != null)
        {
            stickImage.CrossFadeColor(normalColor, 0.1f, true, true);
            if (defaultSprite != null) stickImage.sprite = defaultSprite;
        }

        StickRect.rotation = Quaternion.identity;
        inputVector = Vector2.zero;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || !isUpdate)
            return;

        DeathArea = CenterReference.position;

        if (!isFollowStick)
            return;

        if (!isFree)
            return;

        StickRect.position = Vector3.SmoothDamp(StickRect.position, DeathArea, ref currentVelocity, smoothTime);
        if (Vector3.Distance(StickRect.position, DeathArea) < .1f)
        {
            isFree = false;
            StickRect.position = DeathArea;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (lastId == -2)
        {
            lastId = data.pointerId;
            StopAllCoroutines();
            StartCoroutine(ScaleJoysctick(true));
            OnDrag(data);
            if (backImage != null)
            {
                backImage.CrossFadeColor(pressColor, Duration, true, true);
            }
            if (stickImage != null)
            {
                stickImage.CrossFadeColor(pressColor, Duration, true, true);
                if (clickSprite != null) stickImage.sprite = clickSprite;
            }
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.pointerId == lastId)
        {
            isFree = false;

            Vector3 position = bl_JoystickUtils.TouchPosition(m_Canvas, GetTouchID);
            Vector3 dir = position - DeathArea;

            float dist = dir.magnitude;
            float clampedDist = Mathf.Min(dist, radio);
            Vector3 logicalPos = DeathArea + (dist > 0f ? (dir / dist) * clampedDist : Vector3.zero);

            if (Radio > 0f)
            {
                inputVector = new Vector2(
                    (logicalPos.x - DeathArea.x) / Radio,
                    (logicalPos.y - DeathArea.y) / Radio
                );
            }
            else
            {
                inputVector = Vector2.zero;
            }

            if (isFollowStick)
            {
                if (Vector2.Distance(DeathArea, position) < radio)
                {
                    StickRect.position = position;
                }
                else
                {
                    StickRect.position = DeathArea + dir.normalized * radio;
                }
            }
            else
            {
                StickRect.position = DeathArea;
            }

            if (stickImage != null && dragSprite != null)
            {
                if (stickImage.sprite != dragSprite)
                    stickImage.sprite = dragSprite;
            }

            if (dir.sqrMagnitude > k_MinDirSqr)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                lastAngleDeg = angle;
                StickRect.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            else
            {
                StickRect.rotation = Quaternion.Euler(0f, 0f, lastAngleDeg);
            }
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        isFree = true;
        currentVelocity = Vector3.zero;

        if (data.pointerId == lastId)
            ResetJoystick();
    }

    private void ResetJoystick()
    {
        lastId = -2;
        StopAllCoroutines();
        StartCoroutine(ScaleJoysctick(false));
        inputVector = Vector2.zero;

        if (backImage != null)
        {
            backImage.CrossFadeColor(normalColor, Duration, true, true);
        }
        if (stickImage != null)
        {
            stickImage.CrossFadeColor(normalColor, Duration, true, true);

            if (defaultSprite != null)
                stickImage.sprite = defaultSprite;
        }

        StickRect.rotation = Quaternion.identity;
        if (isFollowStick)
        {
            StickRect.position = DeathArea;
        }
    }

    IEnumerator ScaleJoysctick(bool increase)
    {
        float _time = 0;

        while (_time < Duration)
        {
            Vector3 v = StickRect.localScale;
            if (increase)
            {
                v = Vector3.Lerp(StickRect.localScale, PressScaleVector, (_time / Duration));
            }
            else
            {
                v = Vector3.Lerp(StickRect.localScale, Vector3.one, (_time / Duration));
            }
            StickRect.localScale = v;
            _time += Time.deltaTime;
            yield return null;
        }
    }
}
