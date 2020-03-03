using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVoid : MonoBehaviour, IBossAI {

    public Vector2 movementAreaMin;
    public Vector2 movementAreaMax;
    public float speed;

    public float startingBulletCooldown; //How long to wait between bullets
    public float endingBulletCooldown;
    private float bulletHeat;
    public Vector3 bulletOffset;
    public ObjectPool bulletPool;
    public float bulletSpeed;

    private Creature creature;
    private Rigidbody2D body2d;
    private KilledBy killedBy;

    public float attackFrequency
    {
        set
        {
            startingBulletCooldown = value;
        }
    }

    public float attackStrength
    {
        set
        {
            endingBulletCooldown = startingBulletCooldown / value;
        }
    }

    void Awake()
    {
        bulletPool = GetComponent<ObjectPool>();
        creature = GetComponent<Creature>();
        body2d = GetComponent<Rigidbody2D>();
        killedBy = GetComponent<KilledBy>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        float currentCooldown = Mathf.Lerp(endingBulletCooldown, startingBulletCooldown, (creature.health / creature.maxHealth));

        if(Player.instance.gameObject.activeSelf)
            bulletHeat += Time.deltaTime;

        if (bulletHeat >= currentCooldown)
        {
            GameObject bullet = bulletPool.nextObject(transform.position);
            Vector2 targetingVector = -(MotionUtils.flattenVector(transform.position - Player.instance.transform.position)).normalized;
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletSpeed * targetingVector.x, bulletSpeed * targetingVector.y); //Sets bullet velocity
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, 180 * Mathf.Atan2(targetingVector.y, targetingVector.x) / Mathf.PI);    //Rotates bullet according to velocity
            bulletHeat = 0;
        }

        //Takes care of movement
        if (getOutsideBox())
        {
            //Picks a new point inside the movement box
            float newX = Random.Range(movementAreaMin.x, movementAreaMax.x);
            float newY = Random.Range(movementAreaMin.y, movementAreaMax.y);
            //gets a unit vector from her to the point
            Vector2 dirVector = (new Vector2(newX, newY) - MotionUtils.flattenVector(transform.position)).normalized;
            //sets velocity accordingly
            body2d.velocity = new Vector2(dirVector.x * speed, dirVector.y * speed);
        }

    }

    bool getOutsideBox() //Returns if she's outside her movement zone
    {
        return ((transform.position.x < movementAreaMin.x || transform.position.y < movementAreaMin.y) ||
            (transform.position.x > movementAreaMax.x || transform.position.y > movementAreaMax.y));
    }
}
