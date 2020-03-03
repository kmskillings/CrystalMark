using UnityEngine;
using System.Collections;

public class BossChaos : MonoBehaviour, IBossAI {

	public float attackFrequency {
		set {
            attackCooldown = value;
		}
	}

	public float attackStrength {
		set {
			//pass
		}
	}

	public float xMin; //The minimum x value that Chaos can move to.
	public float xMax; //The maximum x value that Chaos can move to.
	public float y; //The y position that Chaos stays on.

	public float sweepSpeed; //How fast Chaos sweeps across the screen with her beam
    public float chargeSpeed;
	public float normalSpeed; //How fast Chaos goes normally

	public int mode; //Which mode Chaos is in.

	public GameObject redSwirl;
	public GameObject graySwirl;
	public GameObject beam;
	public AudioClip beamSound;

    public GameObject bombPrefab;
    public float bombCooldown;
    private float bombHeat = 0f;
    public Vector2 bombVelocity;
    public AudioClip bombLaunch;

    public Sprite redSwirlRipple;
    public Sprite greySwirlRipple;
    public Sprite damaged;
    public Sprite damagedRipple;

	public float windupTime;
	public float attackCooldown;
	public float attackDuration; //How long the attack actually takes (How fast she charges, how long the lazarr shoots, how fast the bombs fly).
	private float attackHeat = 0;
	private AudioSource audioSource;
	private bool movingIntoPosition = true;
    //windupTime is how long an attack takes to charge. attackCooldown is how long it is between the beginning of an attack cycle.
    //Vulnerable tme is attackCooldown-windupTime-attackDuration

    public delegate void delegateAIPhase();
    public delegateAIPhase currentAIPhase;

	private Rigidbody2D body2d;
    private ObjectPool bombPool;

	void Awake () {
	
		audioSource = GetComponent<AudioSource> ();
		audioSource.volume = 1f;
		body2d = GetComponent<Rigidbody2D> ();

        bombPool = GetComponent<ObjectPool>();

        currentAIPhase = AIPhase1;
	
	}

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {

        if (movingIntoPosition)
        {
            body2d.velocity = new Vector2(0, -normalSpeed);
            if (transform.position.y < y)
            {
                movingIntoPosition = false;
                body2d.velocity = new Vector2(Random.Range(0, 2) == 1 ? -normalSpeed : normalSpeed, 0);
            }
            return;
        }

        //sets alphas of swirls
        redSwirl.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, Mathf.Lerp(0, 1, (windupTime + attackDuration - attackCooldown + attackHeat) / windupTime)); //Trust me on this
        graySwirl.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, Mathf.Lerp(0, 1, (windupTime + attackDuration - attackCooldown + attackHeat) / windupTime)); //Trust me on this

        //Takes care of colliders (protection) for red swirl
        if (attackHeat >= attackCooldown - attackDuration - windupTime)
        {
            redSwirl.GetComponent<CircleCollider2D>().enabled = true;
            GetComponent<KilledBy>().isEnabled = false;
        }
        else
        {
            redSwirl.GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<KilledBy>().isEnabled = true;
        }

        currentAIPhase();
    }

    void AIPhase1()
    {

        //Activates beam
        if (attackHeat >= attackCooldown - attackDuration && !beam.activeSelf)
        {
            beam.SetActive(true);
            //beam.GetComponent<BossChaosBeam> ().fadeIn ();
            audioSource.clip = beamSound;
            audioSource.Play();

            //start sweeping screen
            body2d.velocity = new Vector2(transform.position.x < 0 ? sweepSpeed : -sweepSpeed, 0);

        }

        //Ticks
        attackHeat += Time.deltaTime;
        if (attackHeat >= attackCooldown)
        {
            beam.SetActive(false);
            audioSource.Stop();
            //start new cycle
            attackHeat = 0;

            //starts moving in random direction (+x or -x)
            body2d.velocity = new Vector2(Random.Range(0, 2) == 1 ? -normalSpeed : normalSpeed, 0);
        }

        //checks if she's moving out of bounds
        if (transform.position.x > xMax)
            body2d.velocity = new Vector2(-Mathf.Abs(body2d.velocity.x), 0);
        else if (transform.position.x < xMin)
            body2d.velocity = new Vector2(Mathf.Abs(body2d.velocity.x), 0);

        if(GetComponent<Creature>().health < 2f / 3f * GetComponent<Creature>().maxHealth)
        {
            loadAIPhase2();
        }
    }

    void AIPhase2()
    {
        //starts bomb-throwing sequence
        if (attackHeat >= attackCooldown - attackDuration)
        {
            //checks if bomb is ready yet
            if(bombHeat >= bombCooldown)
            {
                bombHeat = Random.Range(0, bombCooldown); //This adds some randomness to her movement
                
                //throws bomb
                GameObject bomb = bombPool.nextObject(transform.position);
                bomb.GetComponent<Rigidbody2D>().velocity = bombVelocity;
                audioSource.PlayOneShot(bombLaunch);

                //starts moving in new direction
                body2d.velocity = new Vector2(-Mathf.Sign(body2d.velocity.x) * sweepSpeed, 0);
            }
            else
            {
                bombHeat += Time.deltaTime;
            }
        }

        //Ticks
        attackHeat += Time.deltaTime;
        if (attackHeat >= attackCooldown)
        {
            //start new cycle
            attackHeat = 0;

            //starts moving in random direction (+x or -x)
            body2d.velocity = new Vector2(Random.Range(0, 2) == 1 ? -normalSpeed : normalSpeed, 0);
        }

        //checks if she's moving out of bounds
        if (transform.position.x > xMax)
            body2d.velocity = new Vector2(-Mathf.Abs(body2d.velocity.x), 0);
        else if (transform.position.x < xMin)
            body2d.velocity = new Vector2(Mathf.Abs(body2d.velocity.x), 0);

        if (GetComponent<Creature>().health < 1f / 3f * GetComponent<Creature>().maxHealth)
        {
            loadAIPhase3();
        }
    }

    void loadAIPhase2()
    {
        currentAIPhase = AIPhase2;

        body2d.velocity = Vector2.zero;

        redSwirl.GetComponent<SpriteRenderer>().sprite = redSwirlRipple;
        graySwirl.GetComponent<SpriteRenderer>().sprite = greySwirlRipple;
        GetComponent<SpriteRenderer>().sprite = damaged;

        audioSource.PlayOneShot(bombLaunch, 1f);
    }

    void AIPhase3()
    {
        redSwirl.tag = "MonsterBullet";
        //attack phase of cycle
        if(attackHeat > attackCooldown - attackDuration)
        {
            //charges downward
            body2d.velocity = new Vector2(0, -chargeSpeed);
            if(attackHeat > attackCooldown - attackDuration / 2) //Checks if it's the second half of the attack phase
            {
                body2d.velocity = -body2d.velocity;
            }
        }
        else
        {
            //follows player
            body2d.velocity = new Vector2(-Mathf.Sign((transform.position - Player.instance.transform.position).x) * normalSpeed, 0);
        }

        //Ticks
        attackHeat += Time.deltaTime;
        if (attackHeat >= attackCooldown)
        {
            //start new cycle
            attackHeat = 0;
        }
    }

    void loadAIPhase3()
    {
        Debug.Log("loading 3rd phase");
        currentAIPhase = AIPhase3;

        body2d.velocity = Vector2.zero;

        GetComponent<SpriteRenderer>().sprite = damagedRipple;
        audioSource.PlayOneShot(bombLaunch);
        normalSpeed = Player.instance.speed.x;
    }
		
}
