using System;
using System.Collections;
using UnityEngine;

public interface IPlayerAction
{
    bool IsInAction();
}

public class SpellCastingController : MonoBehaviour, IPlayerAction
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform castLocationTransform;
    [SerializeField] private ProjectileSpellDescription simpleAttackSpell;
    private ProjectileSpellDescription specialAttackSpell;
    [SerializeField] private DropCollector dropCollector;


    private bool inAction;
    private bool isSpecialEnabled = false;
    private float lastSimpleAttackTimestamp = -100;
    private float lastSpecialAttackTimestamp = -100;

    public SpellDescription SimpleAttackSpellDescription { get => simpleAttackSpell; }
    public SpellDescription SpecialAttackSpellDescription { get => specialAttackSpell; }

    private void Start()
    {
        Debug.Assert(simpleAttackSpell, "No spell assigned to SpellCastingController.");
        dropCollector.AbilityCollected += OnAbilityCollected;

    }
    private void OnAbilityCollected(ProjectileSpellDescription projectileSpellDescription)
    {
        specialAttackSpell = projectileSpellDescription;
        isSpecialEnabled = true;
    }

    void Update()
    {
        bool simpleAttack = Input.GetButtonDown("Fire1");
        bool specialAttack = Input.GetButtonDown("Fire2");

        if (!inAction)
        {
            if (simpleAttack && GetSimpleAttackCooldown() == 0)
            {
                StartCoroutine(SimpleAttackRoutine());
            }
            else if (specialAttack && isSpecialEnabled && GetSpecialAttackCooldown() == 0)
            {
                StartCoroutine(SpecialAttackRoutine());
            }
        }
    }

    private IEnumerator SpecialAttackRoutine()
    {
        inAction = true;
        animator.SetTrigger(specialAttackSpell.AnimationVariableName);

        yield return new WaitForSeconds(specialAttackSpell.ProjectileSpawnDelay);

        Instantiate(specialAttackSpell.ProjectilePrefab, castLocationTransform.position, castLocationTransform.rotation);

        yield return new WaitForSeconds(specialAttackSpell.Duration - specialAttackSpell.ProjectileSpawnDelay);

        lastSpecialAttackTimestamp = Time.time;
        inAction = false;
    }

    private IEnumerator SimpleAttackRoutine()
    {
        inAction = true;
        animator.SetTrigger(simpleAttackSpell.AnimationVariableName);

        yield return new WaitForSeconds(simpleAttackSpell.ProjectileSpawnDelay);

        Instantiate(simpleAttackSpell.ProjectilePrefab, castLocationTransform.position, castLocationTransform.rotation);

        yield return new WaitForSeconds(simpleAttackSpell.Duration - simpleAttackSpell.ProjectileSpawnDelay);

        lastSimpleAttackTimestamp = Time.time;
        inAction = false;
    }

    public bool IsInAction()
    {
        return inAction;
    }

    public float GetSimpleAttackCooldown()
    {
        return Mathf.Max(0, lastSimpleAttackTimestamp + simpleAttackSpell.Cooldown - Time.time);
    }
    public float GetSpecialAttackCooldown()
    {
        if (specialAttackSpell == null)
        {
            return 0;
        }
        return Mathf.Max(0, lastSpecialAttackTimestamp + specialAttackSpell.Cooldown - Time.time);
    }
}
