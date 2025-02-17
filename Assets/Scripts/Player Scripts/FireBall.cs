//Worked on by : Jacob Irvin

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class FireBall : MonoBehaviour
{
    //the body of our projectile that will handle our physics
    [SerializeField] Rigidbody rb;

    //game variables that may be tweaked
    [SerializeField] float damage;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] DamageStats type;
    [SerializeField] float minimumLight, maximumLight;

    //variable to be used in the lighting
    [SerializeField] new Light light;


    // Start is called before the first frame update
    void Start()
    {
        //moves our projectile forward based on its speed
        rb.velocity = transform.forward * speed;
        //after being alive so long our projectile will die
        if (PhotonNetwork.InRoom)
            StartCoroutine(WaitThenDestroy(gameObject, destroyTime));
        else if (!PhotonNetwork.InRoom)
            Destroy(gameObject, destroyTime);
        light.intensity = Random.Range(minimumLight, maximumLight);
    }

    void Update() 
    {
        light.intensity = Random.Range(light.intensity + 0.2f, light.intensity - 0.2f);
    }    

    IEnumerator WaitThenDestroy(GameObject obj, float destroyTime) {
        yield return new WaitForSeconds(destroyTime);
        PhotonNetwork.Destroy(obj);
    }

    private void OnTriggerEnter(Collider other)
    {
        //when encountering a collision trigger it checks for component IDamage
        IDamage dmg = other.GetComponent<IDamage>();

        //if there is an IDamage component we run the inside code
        if (dmg != null && !other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("PlayerChild"))
        {
            if (SkillTreeManager.Instance.IsSkillUnlocked(SkillTreeManager.Skills.ATTACK_DAMAGE_UP))
                damage *= 1.5f;

            //deal damage to the object hit
            dmg.TakeDamage(damage);
            if (type != null)
                dmg.Afflict(type);
            //destroy our projectile
            DestroyObject();
        }
        else if (!other.gameObject.CompareTag("Player") && !other.isTrigger && !other.gameObject.CompareTag("PlayerChild"))
            DestroyObject();

    }

    void DestroyObject() {
        if (PhotonNetwork.InRoom && GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(gameObject);
        else if (!PhotonNetwork.InRoom)
            Destroy(gameObject);
    }
}
