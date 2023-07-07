using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Image[] _images;
    [SerializeField] private float _speed=1;
    [SerializeField] private float _duration=1;
    private float _timeLeft;

    private void Update()
    {
        transform.position += Vector3.up * _speed * Time.deltaTime;
        if (_timeLeft > 0) { _timeLeft -= Time.deltaTime; }
        else { gameObject.SetActive(false); }
    }

    public void SetNotification(string text, Color color, Sprite sprite)
    {
        transform.rotation = Quaternion.identity;
        _timeLeft = _duration;
        _text.text = text;
        _text.color = color;
        foreach (Image image in _images)
        {
            image.sprite = sprite;
            image.color = color;
        }
    }
}
