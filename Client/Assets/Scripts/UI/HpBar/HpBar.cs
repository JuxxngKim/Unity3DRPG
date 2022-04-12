using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YeongJ.Inagme;

public class HpBar : MonoBehaviour
{
    [SerializeField] Slider _hpSlider;
    [SerializeField] Slider _bgSlider;
    [SerializeField] Text _levelText;
    [SerializeField] Text _nameText;
    [SerializeField] bool _moveLock;

    public BaseActor Owner { get; private set; }

    public void SetActor(BaseActor owner)
    {
        Owner = owner;

        if(_nameText != null)
            _nameText.text = Owner.name;

        if (_levelText != null)
            _levelText.text = $"Lv.{Owner.Stat.Level}";

        ChangeHp(Owner.Stat.Hp);
    }

    public void ChangeHp(int hp)
    {
        float ratio = hp <= 0.0f ? 0.0f : (float)hp / (float)Owner.Stat.MaxHp;
        _hpSlider.value = ratio;
    }

    public void UpdateHpBar()
    {
        if (Owner == null)
            return;

        if(!_moveLock)
        {
            var velocity = Vector3.zero;
            var targetPosition = Camera.main.WorldToScreenPoint(Owner.UIRoot.transform.position);
            this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref velocity, 0.005f);
        }

        if(_hpSlider.value != _bgSlider.value)
        {
            var sliderVelocity = 0.0f;
            _bgSlider.value = Mathf.SmoothDamp(_bgSlider.value, _hpSlider.value, ref sliderVelocity, 0.05f);
        }
    }
}
