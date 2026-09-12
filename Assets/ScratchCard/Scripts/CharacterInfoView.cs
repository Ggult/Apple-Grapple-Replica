using System.Linq;
using AppleGrapple;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Lives under the single shared CharacterInfoCanvas, not under the character — LateUpdate projects the
// follow target's world position into that canvas's local space every frame instead of relying on parenting.
public class CharacterInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickLabel;
    [SerializeField] private Image flagImage;
    [SerializeField] private Sprite[] flagSprites;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);

    private Transform _followTarget;

    public void Setup(CharacterProfileData data)
    {
        nickLabel.text = data.Nickname;
        var flagSprite =  flagSprites.FirstOrDefault(sprite => sprite.name == data.FlagCode);
        if (flagSprite != null)
        {
            flagImage.sprite = flagSprite;
            flagImage.gameObject.SetActive(true);
        }
        else
        {
            flagImage.gameObject.SetActive(false);
        }
    }

    public void UpdateHealth(int current, int max)
    {
        healthBar.UpdateFill(current, max);
    }

    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }

    private void LateUpdate()
    {
        if (_followTarget == null) return;

        var canvas = CharacterInfoCanvas.Instance;
        if (canvas == null) return;

        var screenPoint = canvas.WorldCamera.WorldToScreenPoint(_followTarget.position + worldOffset);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.RectTransform, screenPoint, null, out var localPoint))
        {
            rectTransform.localPosition = localPoint;
        }
    }
}
