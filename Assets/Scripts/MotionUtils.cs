using UnityEngine;
using System.Collections;

public static class MotionUtils{

	public static void steer(GameObject gameObj, float degrees){
		//gets components of gameObj
		Rigidbody2D body2d = gameObj.GetComponent<Rigidbody2D> ();
		if (body2d == null) {
			Debug.Log ("Cannot use any MotionUtil methods without a RigidBody2D component.");
			return;
		}

		//determines offset of the object's trajectory to 0 degrees (straight right)
		float rotOffset = 180*Mathf.Atan2(body2d.velocity.y,body2d.velocity.x)/Mathf.PI;
		//rotates object's tranform (but not its velocity)
		gameObj.transform.rotation = Quaternion.Euler(gameObj.transform.rotation.eulerAngles + new Vector3(0, 0, degrees));
		//rotates velocity
		float rotTotal = (rotOffset + degrees) * Mathf.PI / 180; //IMPORTANT: This is in radians
		float hypotenuse = body2d.velocity.magnitude;
		float tmpVelX = Mathf.Cos (rotTotal) * hypotenuse; //Calculates sine and cosine of rotTotal, uses them to 
		float tmpVelY = Mathf.Sin (rotTotal) * hypotenuse; //calculate velocity components
		body2d.velocity = new Vector2(tmpVelX, tmpVelY);
	}

	public static void steerTowards(GameObject gameObj, Transform target, float maxDegrees){
		//steers towards a target, with a maximum steering rotation of + or -maxDegrees degrees per tick
		//gets components
		Rigidbody2D body2d = gameObj.GetComponent<Rigidbody2D> ();
		if (body2d == null) {
			Debug.Log ("Cannot use any MotionUtil methods without a RigidBody2D component.");
			return;
		}

		//determines angle that gameObj is flying at. ALL  UNITS ARE DEGREES
		float angleVel = 180 * Mathf.Atan2 (body2d.velocity.y, body2d.velocity.x) / Mathf.PI;
		//determines angle to target
		float angleTarget = MotionUtils.getAngleTo (gameObj.transform.position, target.position);
		//steers toward target with a rotation between + and - degrees
		steer(gameObj, Mathf.Clamp(angleTarget - angleVel, -maxDegrees, maxDegrees));
	}

	public static float getAngleTo(Vector3 origin, Vector3 target){
		return getAngleTo (new Vector2 (origin.x, origin.y), new Vector2 (target.x, target.y));
	}

	public static float getAngleTo(Vector2 origin, Vector2 target){
		float adjustedY = target.y - origin.y;
		float adjustedX = target.x - origin.x;
		float angleTo = 180 * Mathf.Atan2 (adjustedY, adjustedX) / Mathf.PI; //Finds angle to target in degrees
		//adjusts for error
		return angleTo;
	}

	public static Vector2 flattenVector(Vector3 vector){
		//creates a Vector2 with the x and y of a Vector3
		return new Vector2(vector.x, vector.y);
	}

	public static Vector3 unflattenVector(Vector2 vector){
		return new Vector3 (vector.x, vector.y, 0);
	}
}
