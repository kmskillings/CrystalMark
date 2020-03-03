using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPrejudice : MonoBehaviour, IBossAI {

    public Vector2 movementAreaMin;
    public Vector2 movementAreaMax;
    public float speed;

    public float chargeTime;
    public float attackCooldown;
    private float attackHeat;
    public float pulseTime;

    public Color chargeColor;
    public Color pulseColor;

    private GameObject gate; //Only one gate will ever be present at a time
    public GameObject gatePrefab;
    public float gateMaxScale; //How much larger the gate will be than its normal at its largest
    private SpriteRenderer gateRender;

    private Rigidbody2D body2d;
    private SpriteRenderer render;
    private KilledBy killedBy;

    public float attackFrequency
    {
        set
        {
            attackCooldown = value;
            chargeTime = 5f * attackCooldown / 8f; //The charge time is 5 eighths of the total time
            pulseTime = attackCooldown / 8f;
        }
    }

    //She is vulnerable for 1/value of the total attack time. The pulse time is equivalent to 1/2 of the vulnerable time
    public float attackStrength
    {
        set
        {
            gateMaxScale = value; //This higher this value, the larger the gate grows
        }
    }


    void Awake()
    {

        body2d = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        killedBy = GetComponent<KilledBy>();

        //sets up gate
        gate = Instantiate(gatePrefab);

        gateRender = gate.GetComponent<SpriteRenderer>();

        //sets up counters
        attackHeat = 0;

    }
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //ticks counters
        attackHeat += Time.deltaTime;
        if(attackHeat >= attackCooldown)
        {
            //resets cycle
            attackHeat = 0;
            killedBy.damage = 1f;
            killedBy.points = 10;
            gate.tag = "Untagged";
            gate.transform.localScale = new Vector3(1f, 1f, 1f);

        }

        //takes care of killedBy
        if(attackHeat >= attackCooldown - chargeTime - pulseTime && killedBy.damage != 0f) //If the boss is no longer vulnerable
        {
            killedBy.damage = 0f;
            killedBy.points = 0;

            //Gate spawns on top of the player
            gate.transform.position = new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, gate.transform.position.z);
        }

        //sets charging colors
        render.color = Color.Lerp(Color.white, chargeColor, (pulseTime + chargeTime - attackCooldown + attackHeat) / chargeTime);
        gateRender.color = Color.Lerp(Color.clear, Color.white, (pulseTime + chargeTime - attackCooldown + attackHeat) / chargeTime);

        if (attackHeat > attackCooldown - pulseTime) //Takes care of pulse colors
        {
            render.color = Color.Lerp(Color.white, pulseColor, (pulseTime - attackCooldown + attackHeat) / pulseTime);

            gate.tag = "MonsterBullet";
            float gateScale = Mathf.Lerp(1, gateMaxScale, (pulseTime - attackCooldown + attackHeat) / pulseTime);
            gate.transform.localScale = new Vector3(gateScale, gateScale, 1);
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
