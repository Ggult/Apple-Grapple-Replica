using System.Linq;
using AppleGrapple;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickLabel;
    [SerializeField] private Image flagImage;
    [SerializeField] private Sprite[] flagSprites;
    public void Setup(CharacterData data)
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
}
