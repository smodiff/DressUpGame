using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject _blushPage;
    [SerializeField] private Image _blushButtonImage;
    [SerializeField] private Sprite _blushDefault;
    [SerializeField] private Sprite _blushActive;

    [Space(10)]
    [SerializeField] private GameObject _eyePage;
    [SerializeField] private Image _eyeButtonImage;
    [SerializeField] private Sprite _eyeDefault;
    [SerializeField] private Sprite _eyeActive;

    [Space(10)]
    [SerializeField] private GameObject _lipstickPage;
    [SerializeField] private Image _lipstickButtonImage;
    [SerializeField] private Sprite _lipstickDefault;
    [SerializeField] private Sprite _lipstickActive;

    [Space(10)]
    [SerializeField] private HandController _handController;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ResetBookPages();
        OpenBlushPage();
    }

    private void ResetBookPages()
    {
        
        _blushButtonImage.sprite = _blushDefault;
        _eyeButtonImage.sprite = _eyeDefault;
        _lipstickButtonImage.sprite = _lipstickDefault;

        _blushPage.SetActive(false);
        _eyePage.SetActive(false);
        _lipstickPage.SetActive(false);
        _handController.DropBrush();
    }

    public void OpenBlushPage()
    {
        ResetBookPages();
        _blushButtonImage.sprite = _blushActive;
        _blushPage.SetActive(true);
    }

    public void OpenEyeShadowsPage()
    {
        ResetBookPages();
        _eyeButtonImage.sprite = _eyeActive;
        _eyePage.SetActive(true);
    }

    public void OpenLipstickPage()
    {
        ResetBookPages();
        _lipstickButtonImage.sprite = _lipstickActive;
        _lipstickPage.SetActive(true);
    }
}
