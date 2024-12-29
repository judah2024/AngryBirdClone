using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
	[Header("파괴")]
	[SerializeField] protected float maxHealth = 100f;
	[SerializeField] protected int scoreValue = 100;
	[SerializeField] protected float damageMultiplier = 1f;	// 이후, 물체 마다 다른 데미지 부여 (유리 : 잘 깨짐, 철 : 단단함)
	
	// SoundManager에서 관리
	// [Header("Effects")]
	// [SerializeField] protected GameObject destructionEffectPrefab;
	// [SerializeField] protected AudioClip hitSound;
	// [SerializeField] protected AudioClip destroySound;
	
	protected float CurrentHealth;
	protected bool IsDestroyed = false;
	protected SpriteRenderer _renderer;

	public float RatioHp => CurrentHealth / maxHealth;

	protected virtual void Awake()
	{
		CurrentHealth = maxHealth;
		_renderer = GetComponent<SpriteRenderer>();
	}
	
	public virtual void TakeDamage(float damage, Vector2 impactPoint)
	{
		if (IsDestroyed)
			return;

		float actualDamage = damage * damageMultiplier;
		CurrentHealth = Mathf.Max(CurrentHealth - actualDamage, 0);

		Debug.Log($"{gameObject.name} : {CurrentHealth}");
		if (CurrentHealth == 0)
		{
			DestroyObject();
		}
	}

	protected virtual void DestroyObject()
	{
		IsDestroyed = true;
		
		GameManager.Instance.AddScore(scoreValue);
		
		Destroy(gameObject);
	}

	protected void OnCollisionEnter2D(Collision2D other)
	{
		if (IsDestroyed)
			return;

		if (GameManager.Instance.isLaunched == false)
			return;
		
		float power = other.relativeVelocity.magnitude;
		TakeDamage(1, other.GetContact(0).point);
	}
}
