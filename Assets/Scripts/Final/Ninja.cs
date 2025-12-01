using Microsoft.Unity.VisualStudio.Editor;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EndGame;


public class Ninja : Character
{
    Animator animator;
    [SerializeField] TextMeshProUGUI healthTxt, damageTxt;
    // [SerializeField] TextMeshProUGUI healthTxt, appleTxt, cherryTxt, damageTxt;


    protected int apple = 0;
    public int Apple => apple;

    protected float cherry = 0;
    public float Cherry => (float)cherry;


    HashSet<GameObject> nearbyMonster = new();
    public GameObject AppleSpawn, CherrySpawn;

    [HideInInspector] public MapGenerator mapGenerator;


    void Start()
    {
        Damage = 10;
        Health = 100;
        UpdateHelthTxt();
        UpdateDamageTxt();
        // UpdateAppleTxt();
        // UpdateCherryTxt();
        animator = GetComponent<Animator>();
    }


    // Keep Apple & Cherry
    public void KeepUp(int count)
    {
        // Keep Apple
        apple += count;
        Health += 20;
        // UpdateAppleTxt();
        // UpdateHelthTxt();
        Debug.Log($"+ Keep Potion 1 [ Hp : {Health} ]");
    }

    public void KeepUp(float count)
    {
        // Keep Cherry
        cherry += count;
        Damage += 5;
        UpdateDamageTxt();
        // UpdateCherryTxt();
        Debug.Log($"+ Keep Potion 2 [ Damage : {Damage} ]");
    }


    // When Player Near Monster & Out Monster
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (!collision.CompareTag("Monster")) return;

        GameObject monster = collision.gameObject;

        // กันเพิ่มซ้ำ
        if (!nearbyMonster.Contains(monster))
        {
            nearbyMonster.Add(monster);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null) return;
        if (!collision.CompareTag("Monster")) return;

        GameObject monster = collision.gameObject;

        // กัน error ถ้า monster ถูกลบไปแล้ว
        if (nearbyMonster.Contains(monster))
        {
            nearbyMonster.Remove(monster);
        }
    }



    // Player Left-Click to Attack
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AnimationAttack();
        }
    }


    public override void AnimationAttack()
    {
        Attack();
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("NinjaAttack"))
        {
            animator.SetTrigger("Attack");
            Attack();
        }
    }


    void Attack()
    {
        List<GameObject> monstersToAttack = new List<GameObject>(nearbyMonster);
        foreach (GameObject ninjaObject in monstersToAttack)
        {
            Character monster = ninjaObject.GetComponent<Character>();

            if (monster != null)
            {
                monster.TakeDamage(Damage);

                if (monster.Health <= 0)
                {
                    Destroy(monster.gameObject);
                    SpawnNewObject(monster.transform.position);
                    nearbyMonster.Remove(ninjaObject);
                    GetExitSpawnChancePercent();
                }
            }
        }
    }


    private void Awake()
    {
        // กำหนด MapGenerator อัตโนมัติ
        mapGenerator = FindObjectOfType<MapGenerator>();

        if (mapGenerator == null)
            Debug.LogError("MapGenerator not found in the scene!");
    }

    private void GetExitSpawnChancePercent()
    {
        if (mapGenerator == null)
        {
            Debug.LogWarning("mapGenerator is NULL — cannot spawn exit.");
            return;
        }

        if (Random.Range(0, 100) < mapGenerator.exitSpawnChancePercent)
        {
            mapGenerator.PlaceExit();
        }
    }






    // Spwn Cherry When Monster Dead

    private void SpawnNewObject(Vector3 spawnPosition)
    {
        // Spawn Apple
        if (AppleSpawn != null)
        {
            spawnPosition.x += Random.Range(2f, 5f);
            spawnPosition.y += Random.Range(1.0f, 1.5f);
            Instantiate(AppleSpawn, spawnPosition, Quaternion.identity);
        }

        // Spawn Cherry
        if (CherrySpawn != null)
        {
            spawnPosition.x += Random.Range(2f, 5f);
            spawnPosition.y += Random.Range(1.0f, 1.5f);
            Instantiate(CherrySpawn, spawnPosition, Quaternion.identity);
        }
    }




    // Update User Interface
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        UpdateHelthTxt();
    }

    void UpdateHelthTxt()
    {
        healthTxt.text = $"Hp : {Health}";

        if (Health <= 0)
        {
            Debug.Log("Player Dead!");
            GameResult.isWin = false;
            SceneManager.LoadScene("EndGame");
        }
    }


    void UpdateDamageTxt()
    {
        damageTxt.text = $"Damage : {Damage}";
    }


    void UpdateAppleTxt()
    {
        // appleTxt.text = $"{Apple}";
    }

    void UpdateCherryTxt()
    {
        // cherryTxt.text = $"{Cherry}";
    }

}
