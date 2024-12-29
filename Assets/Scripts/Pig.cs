using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : DestructibleObject
{
    [Header("돼지")]
    [SerializeField] private float damageThreshold = 5f;
    [SerializeField] private Sprite damagedSprite;
    [SerializeField] private float stunDuration = 1f;

    private Sprite normalSprite;
    private bool isStunned = false;
    private float stunTimer = 0f;

    private void Awake()
    {
        base.Awake();
        normalSprite = _renderer.sprite;
    }

    void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                RecoverStun();
            }
        }
    }

    public override void TakeDamage(float damage, Vector2 impactPoint)
    {
        if (damage < damageThreshold)
        {
            Stun();
            return;
        }
        
        base.TakeDamage(damage, impactPoint);
    }

    void Stun()
    {
        if (isStunned)
            return;
        
        isStunned = true;
        stunTimer = stunDuration;
        
        // 기절 효과 추가
        //_renderer.sprite = damagedSprite;
    }

    void RecoverStun()
    {
        isStunned = false;
        _renderer.sprite = normalSprite;
    }
}
