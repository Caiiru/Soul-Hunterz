using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Melee_DummyEnemy : Enemy
{
    public float attackCooldown = 5f;
    private float _attackTimer = 0f;
    private bool _canAttack = false;

    [Header("VFX")]
    public Transform SwordJoint;

    private Animator _animator;
    public List<SlashVFX> slashes;
    public int currentSlash = 0;
    protected override void OnEnable()
    {
        base.OnEnable();
        _animator = GetComponent<Animator>();
        VFXDebugManager.OnInputPressed += OnInputPressed;

    }

    private void OnInputPressed(int key)
    {
        if (key == 0)
        {
            _canAttack = true;
        }
        if (key == 1)
        {
            _canAttack = false;
        }
    }

    // Update is called once per frame

    protected void Update()
    { 
        HandleTimer();
    }

    private void HandleTimer()
    {

        if (!_canAttack) return;
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= attackCooldown)
        {
            Attack();
            _attackTimer = 0;
        }

    }
    protected override void Attack()
    {
        
        _animator.SetTrigger("Attack");

    }

    IEnumerator SpawnSlash()
    {
        if(slashes[currentSlash]==null){
            yield break; 
        }
        GameObject _slash = Instantiate(slashes[currentSlash].SlashPrefab, transform.position, SwordJoint.rotation);
        _slash.GetComponentInChildren<VisualEffect>().Stop();
        yield return new WaitForSeconds(slashes[currentSlash].delay);
        _slash.GetComponentInChildren<VisualEffect>().Play();
        Destroy(_slash, 2f);

    }

}

[System.Serializable]
public class SlashVFX
{
    public GameObject SlashPrefab;
    public float delay;
}
