using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public abstract class Character : MonoBehaviour
{
    public Image HealthBarUI;
    protected int health;
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = Mathf.Clamp(value, 0, 100);
            UpdateHpUI();
        }
    }


    protected int damage;
    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = Mathf.Clamp(value, 0, 50);
        }
    }


    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
    }


    void UpdateHpUI()
    {
        if (HealthBarUI != null)
        {
            HealthBarUI.fillAmount = health / 100f;
        }
    }


    public abstract void AnimationAttack();
}
