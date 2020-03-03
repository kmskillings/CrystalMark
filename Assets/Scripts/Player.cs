using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface ICharacter{
	void onFireKey();

	Color heartColor { get; }
}

public class Player : MonoBehaviour {

	//controls for motion and shooting
	public string keyUp = "w";
	public string keyDown = "s";
	public string keyLeft = "a";
	public string keyRight = "d";

    public string keyUpAlt = "up";
    public string keyDownAlt = "down";
    public string keyLeftAlt = "left";
    public string keyRightAlt = "right";

    public string keyFire = "space";
	public string keyFocus = "left shift";

	public int lives = 3; //How many lives the player has left. When lives hits 0, the player dies. Permanently.

	//motion options
	public Vector2 speed;
	public float focusFactor = 1; //how much more slowly should the player move while focusing?

	public bool canMoveOffscreen; //Should the player be allowed to move offscreen?
	public bool useCollider; //Should the player's collider collide with edges of the screen, or its sprite?

	public static Player instance; //A pointer to the player, for other scripts to use

	//borders of the space that the player can move freely in
	public static Vector2 boundsMin;
	public static Vector2 boundsMax;

    public float invulnCooldown; //How long after a hit the character's invulnerable for
    public float invulnHeat; //How long since the last hit. Compare invulnHeat >= invulnCooldown
    private bool invuln; //Wether or not the character takes damage

    public int points;
    public int coins = 1;
    public string pointsPrefix = "Score: ";

	//components
	protected Rigidbody2D body2d;
	protected BoxCollider2D box2d;
    private SpriteRenderer render;

	public delegate void delegateFireKey();
	public event delegateFireKey eventFireKey;

	public bool controllable = true; //Whether or not the player can currently be controlled

	private ICharacter character;

	void Awake () {
		if (instance == null) {
			instance = this; //If there is no instance yet, set instance to this	
		} else if (instance != this) {
			Destroy (gameObject); //If instance is something other than this, destroy this
		}

		DontDestroyOnLoad (this);

		//Gets components
		body2d = GetComponent<Rigidbody2D> ();
		box2d = GetComponent<BoxCollider2D> ();
        render = GetComponent<SpriteRenderer>();

		//gets ICharacters attached to this component and adds their onfireKey events to eventFireKey
		MonoBehaviour[] scripts = GetComponents<MonoBehaviour> ();
		foreach (MonoBehaviour script in scripts) {
			if (script is ICharacter) {
				eventFireKey += (script as ICharacter).onFireKey;
				character = script as ICharacter;
			}
		}

		//gets KilledBys attached to this component and subscribes to their eventDie. Also disableDeath 's
		KilledBy[] killedBys = GetComponents<KilledBy> ();
		foreach (KilledBy killedBy in killedBys) {
			killedBy.enableDeath = false;
			killedBy.eventDie += loseLife;
		}
	}	

	// Use this for initialization
	void Start () {

		//calculate bounds of screen, to limit player
		boundsMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)); //These lines implicitly assume that the player is 10 vertical units away from the camera
		boundsMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10)); //TODO: avoid making this assumption

		//sets colors of life hearts
		foreach (Image life in CanvasPointers.instance.lives) {
			life.color = character.heartColor;
		}

	}
	
	// Update is called once per frame
	void Update () {


		//ticks recovery invulnerability
        if(invulnHeat > invulnCooldown && invuln)
        {
            invuln = false;
            render.color = Color.white;
            foreach (KilledBy killedBy in GetComponents<KilledBy>()) //Reactivates killedBys
                killedBy.isEnabled = true;
        }
        else
        {
            invulnHeat += Time.deltaTime;
        }
        
        //The velocity components currently being calculated
		float tmpVelX = 0;
		float tmpVelY = 0;

		//checks for buttons down and changes velocity components accordingly
		//TODO: make this more graceful, if possible
		if (Input.GetKey (keyUp) || Input.GetKey(keyUpAlt)) {
			tmpVelY += speed.y;
		}
		if (Input.GetKey (keyDown) || Input.GetKey(keyDownAlt)) {
			tmpVelY -= speed.y;
		}
		if (Input.GetKey (keyLeft) || Input.GetKey(keyLeftAlt)) {
			tmpVelX -= speed.x;
		}
		if (Input.GetKey (keyRight) || Input.GetKey(keyRightAlt)) {
			tmpVelX += speed.x;
		}

		//determines if player is focusing. If they are, reduce speed.
		if (Input.GetKey (keyFocus)) {
			tmpVelX *= focusFactor;
			tmpVelY *= focusFactor;
		}

		//determines if player moving against any walls. If they are, speed is zero in that direction.
		//several known bugs with this system exist. They will eventually be fixed.
		//the offset from the center of the player to where they collide with the screen walls. It's symetrical, so use -xOffset for left and xOffset for right, etc.
		float xOffset = 0;
		float yOffset = 0;
		if (useCollider) { //make it so the offsets from the collider are used, if enabled
			xOffset = box2d.size.x/2;
			yOffset = box2d.size.y/2;
		}
		if ((transform.position.x + xOffset > boundsMax.x && tmpVelX > 0) || //If the player is heading off the right side of the screen
			(transform.position.x - xOffset < boundsMin.x && tmpVelX < 0)) { //or the left side
			tmpVelX = 0; //Stop the player
		}
		if ((transform.position.y + yOffset > boundsMax.y && tmpVelY > 0) || //If the player is heading off the top side of the screen
			(transform.position.y - yOffset < boundsMin.y && tmpVelY < 0)) { //or the bottom side
			tmpVelY = 0; //Stop the player
		}

		//If the player isn't controllable, the velocity is always 0
		if (!controllable) {
			tmpVelX = 0;
			tmpVelY = 0;
		}

		//applies calculated velocity
		body2d.velocity = new Vector2(tmpVelX, tmpVelY);

		//Fires firing event
		if (Input.GetKey (keyFire) && controllable) {
			if(eventFireKey != null)
				eventFireKey ();
		}
	}

	public void loseLife() {
		lives -= 1;

		//Sets all the life images to inactive
		foreach (Image lifeImage in CanvasPointers.instance.lives) {
			lifeImage.enabled = false;
		}
		//Goes through and sets the first 'lives' to active
		for (int i = 0; i < lives && i < CanvasPointers.instance.lives.Length; i++) {
			CanvasPointers.instance.lives [i].enabled = true;
		}

		//ends the game if lives are run out of
		if (lives == 0) {
			CanvasPointers.instance.deathImage.gameObject.SetActive (true);
			foreach (KilledBy killedBy in GetComponents<KilledBy>())
				killedBy.isEnabled = false;
			controllable = false;
			EggManager.pause (); //Pauses the eggManager

			gameObject.SetActive (false); //Deactivates the player (For now)
		}

        //Makes character invulnerable for invulnCooldown
        invuln = true;
        invulnHeat = 0f;
        render.color = new Color(255, 255, 255, 0.63f);
        foreach (KilledBy killedBy in GetComponents<KilledBy>()) //Deactivates killedbys
            killedBy.isEnabled = false;
    }

	public void resetLives() {
		if(EggManager.instance != null)
			EggManager.unPause ();
		lives = 3;
		controllable = true;
		CanvasPointers.instance.deathImage.gameObject.SetActive (false);
		gameObject.SetActive (true); //Reactivates the player
		//Activates all the life counters
		foreach (Image lifeImage in CanvasPointers.instance.lives) {
			lifeImage.enabled = true;
		}
		foreach (KilledBy killedBy in instance.GetComponents<KilledBy>())
			killedBy.isEnabled = true;
    }

	void OnLevelWasLoaded(){
		resetLives ();
		foreach (Image life in CanvasPointers.instance.lives) {
			life.color = character.heartColor;
		}

		CanvasPointers.instance.deathImage.GetComponentInChildren<Button>(true).onClick.AddListener(() => { resetLives (); } );
        CanvasPointers.instance.deathImage.GetComponentInChildren<Button>(true).onClick.AddListener(() => { addCoin(); });
        CanvasPointers.instance.pointsText.text = pointsPrefix + instance.points.ToString();
        Debug.Log("Set points to " + instance.points.ToString());
    }

    public void addPoints(int pointsToAdd)
    {
        points += pointsToAdd;
        CanvasPointers.instance.pointsText.text = pointsPrefix + points.ToString();

    }

    public void addCoin()
    {
        instance.coins++;
    }
}