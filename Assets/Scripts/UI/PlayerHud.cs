using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private SpellCastingController spellCastingController;
    [SerializeField] private DropCollector dropCollector;

    [SerializeField] private Image spellIcon;
    [SerializeField] private TMPro.TMP_Text spellCooldownText;
    [SerializeField] private Image secondSpellIcon;
    [SerializeField] private TMPro.TMP_Text secondSpellCooldownText;
    [SerializeField] private TMPro.TMP_Text pickUPText;
    [SerializeField] private GameObject secondAblity;
    [SerializeField] private float fadingSpeed = 0.5f;
    [SerializeField] private float shrinkingSpeed = 5;
    [SerializeField] private GameObject collectUIObject;
    private Coroutine currenCoroutine;
    private float startFontSize;
    private Color startTextColor;
    private bool isSecondAbilityEnabled = false;

    private void Start()
    {
        Debug.Assert(spellCastingController != null, "SpellCastingController reference is null");
        Debug.Assert(dropCollector != null, "DropCollector reference is null");

        spellIcon.sprite = spellCastingController.SimpleAttackSpellDescription.SpellIcon;

        dropCollector.DropsInRangeChanged += OnDropsInRangeChanged;
        dropCollector.DropCollected += OnPickUpLoot;
        dropCollector.AbilityCollected += OnAbilityCollected;
        startFontSize = pickUPText.fontSize;
        startTextColor = pickUPText.color;
    }

    private void OnAbilityCollected(ProjectileSpellDescription obj)
    {
        isSecondAbilityEnabled = true;
        secondSpellIcon.sprite = obj.SpellIcon;
        secondAblity.SetActive(true);

    }

    private void OnPickUpLoot(Drop drop)
    {
        if (currenCoroutine != null)
        {
            StopCoroutine(currenCoroutine);
        }
        ResetPickUpTextValues();
        pickUPText.text = drop.PickUpText;
        StartCoroutine(FadeOutText(pickUPText));
    }

    private void ResetPickUpTextValues()
    {
        pickUPText.text = "";
        pickUPText.fontSize = startFontSize;
        pickUPText.color = startTextColor;
    }

    private IEnumerator FadeOutText(TMPro.TMP_Text text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * fadingSpeed));
            if (text.fontSize > 1f)
            {
                text.fontSize -= Time.deltaTime * shrinkingSpeed;
            }
            yield return null;
        }

    }

    private void OnDropsInRangeChanged()
    {
        collectUIObject.SetActive(dropCollector.DropsInRangeCount > 0);
    }

    private void Update()
    {
        float cooldown = spellCastingController.GetSimpleAttackCooldown();

        if (cooldown > 0)
        {
            spellCooldownText.text = cooldown.ToString("0.0");
            spellIcon.color = new Color(0.25f, 0.25f, 0.25f, 1);
        }
        else
        {
            spellCooldownText.text = "";
            spellIcon.color = Color.white;
        }
        if (isSecondAbilityEnabled)
        {
            float secondCooldown = spellCastingController.GetSpecialAttackCooldown();
            if (secondCooldown > 0)
            {
                secondSpellCooldownText.text = secondCooldown.ToString("0.0");
                secondSpellIcon.color = new Color(0.25f, 0.25f, 0.25f, 1);
            }
            else
            {
                secondSpellCooldownText.text = "";
                secondSpellIcon.color = Color.white;
            }
        }
    }
}
