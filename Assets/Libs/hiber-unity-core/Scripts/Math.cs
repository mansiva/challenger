using UnityEngine;
using System.Collections;

namespace Hibernum 
{
	public static class Math 
	{
		/// <summary>
		/// Two non-parallel lines which may or may not touch each other have a point on each line which are closest
		/// to each other. This function finds those two points. If the lines are not parallel, the function 
		/// outputs true, otherwise false.
		/// </summary>
		/// <returns><c>true</c>the lines are not parallel and the points have been set<c>false</c>lines are parallel, points are Vector3.zero</returns>
		/// <param name="closestPointLine1">return closest point line1</param>
		/// <param name="closestPointLine2">return closest point line2</param>
		/// <param name="linePoint1">line1 origin</param>
		/// <param name="lineVec1">line1 direction</param>
		/// <param name="linePoint2">line2 origin</param>
		/// <param name="lineVec2">line2 direction</param>
		public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){
			
			closestPointLine1 = Vector3.zero;
			closestPointLine2 = Vector3.zero;
			
			float a = Vector3.Dot(lineVec1, lineVec1);
			float b = Vector3.Dot(lineVec1, lineVec2);
			float e = Vector3.Dot(lineVec2, lineVec2);
			
			float d = a*e - b*b;
			
			// lines are not parallel we can get the points
			if (d != 0.0f)
			{
				Vector3 r = linePoint1 - linePoint2;
				float c = Vector3.Dot(lineVec1, r);
				float f = Vector3.Dot(lineVec2, r);
				
				float s = (b*f - c*e) / d;
				float t = (a*f - c*b) / d;
				
				closestPointLine1 = linePoint1 + lineVec1 * s;
				closestPointLine2 = linePoint2 + lineVec2 * t;
				
				return true;
			}
			else {
				// The lines are parallel
				return false;
			}
		}

		/// <summary>
		/// Calculates a signed (+ or - sign instead of being ambiguous) dot product. It is basically used
		/// to figure out whether a vector is positioned to the left or right of another vector. The way this is done is
		/// by calculating a vector perpendicular to one of the vectors and using that as a reference. This is because
		/// the result of a dot product only has signed information when an angle is transitioning between more or less
		/// then 90 degrees.
		/// </summary>
		public static float SignedDotProduct(Vector3 lhs, Vector3 rhs, Vector3 normal)
		{
			// calculate the dot product between the perpendicular vector and the other input vector
			return Vector3.Dot(Vector3.Cross(normal, lhs), rhs);
		}

		/// <summary>
		/// Returns the intersection of the ray and the plane (distance + normal)
		/// </summary>
		/// <returns>Point</returns>
		/// <param name="ray">Ray to cast</param>
		/// <param name="distance">Distance from origin to the plane</param>
		/// <param name="normal">Normal of the plane</param>
		public static Vector3 PointOnPlane(Ray ray, float distance, Vector3 normal) 
		{
			float angle = Vector3.Angle(ray.direction, -normal);
			float hypotenuse = distance / Mathf.Cos(angle * Mathf.Deg2Rad);
			return ray.origin + ray.direction * hypotenuse;
		}

		/// <summary>
		/// Returns the point on the ray at given absolute world height
		/// </summary>
		/// <returns>Point</returns>
		/// <param name="ray">Ray to cast</param>
		/// <param name="height">Absoulte world height</param>
		public static Vector3 PointAtHeight(Ray ray, float height)
		{
			// cast against the "ground" plane
			return PointOnPlane(ray, ray.origin.y - height, Vector3.up);
		}

		/// <summary>
		/// Returns the direction of the angle
		/// </summary>
		/// <returns>-1 for left, 1 for right and 0 for same</returns>
		public static float AngleDirection(Vector3 lhs, Vector3 rhs, Vector3 up) 
		{
			float d = SignedDotProduct(lhs, rhs, up);
			
			if (d > 0f) {
				return 1f;
			} else if (d < 0f) {
				return -1f;
			} else {
				return 0f;
			}
		}
		
		public static float SignedAngle(Vector3 direction) 
		{
			return SignedAngle(Vector3.forward, direction, Vector3.up);	
		}
		
		public static float SignedAngle(Vector3 forward, Vector3 direction, Vector3 up)
		{
			return Vector3.Angle(forward, direction) * AngleDirection(forward, direction, up);	
		}
		
		public static float Angle360(Vector3 direction) 
		{
			return Angle360(Vector3.forward, direction, Vector3.up);	
		}
		
		public static float Angle360(Vector3 forward, Vector3 direction, Vector3 up)
		{
			float angle = Vector3.Angle(forward, direction) * AngleDirection(forward, direction, up);	
			if (angle < 0) {
				angle += 360;	
			}
			
			return angle;
		}
		
		public static float ClampAngle (float a, float min, float max)
		{	
			if (a < -360.0f)
				a += 360.0f;
			if (a > 360.0f)
				a -= 360.0f;
			
			return Mathf.Clamp(a, min, max);
		}

		/// <summary>
		/// Orients the given cube
		/// </summary>
		/// <param name="c1">Front bottom left (0,0,0) (-x, -y, -z)</param>
		/// <param name="c2">Top back right    (1,1,1) (+x, +y, +z).</param>
		public static void OrientCube(ref Vector3 c1, ref Vector3 c2)
		{
			if (c2.x < c1.x)
			{
				float x = c1.x;
				c1.x = c2.x;
				c2.x = x;
			}
			
			if (c2.y < c1.y)
			{
				float y = c1.y;
				c1.y = c2.y;
				c2.y = y;
			}
			
			if (c2.z < c1.z)
			{
				float z = c1.z;
				c1.z = c2.z;
				c2.z = z;
			}
		}
		
		public static float sq(float v) { return v * v; }
		/// <summary>
		/// Check if cube intersects the given sphere
		/// </summary>
		/// <returns><c>true</c>, if intersect sphere was cubed, <c>false</c> otherwise.</returns>
		/// <param name="c1">Front bottom left (0,0,0) (-x, -y, -z)</param>
		/// <param name="c2">Top back right    (1,1,1) (+x, +y, +z)</param>
		/// <param name="origin">Origin.</param>
		/// <param name="r">The red component.</param>
		/// 
		/// c1.x is the left-most face of the cube, 
		///	c2.x is the right-most face, 
		///	c1.y is the bottom-most face, 
		///	c2.y is the top-most face, 
		///	c1.z is the farthest face, 
		///	c2.z is the nearest face
		public static bool CubeIntersectSphere(Vector3 c1, Vector3 c2, Vector3 origin, float r)
		{
			float dist = r * r;
			/* assume c1 and c2 are element-wise sorted, if not, do that now */
			if (origin.x < c1.x) 
				dist -= sq(origin.x - c1.x);
			else if (origin.x > c2.x) 
				dist -= sq(origin.x - c2.x);
			if (origin.y < c1.y) 
				dist -= sq(origin.y - c1.y);
			else if (origin.y > c2.y) 
				dist -= sq(origin.y - c2.y);
			if (origin.z < c1.z) 
				dist -= sq(origin.z - c1.z);
			else if (origin.z > c2.z) 
				dist -= sq(origin.z - c2.z);
			
			return dist > 0;
		}
		
		/// <summary>
		/// Transforms the given world point into the closest viewport point
		/// </summary>
		public static Vector2 ClosestPointInViewport(Vector3 worldPoint) {
			
			if (null == Camera.main)
				return Vector2.zero;
			
			// Get our direction in viewport space from the center of the screen
			Vector2 direction = Camera.main.WorldToViewportPoint(worldPoint)-new Vector3(0.5f,0.5f,0);
			
			// Is our target behind or infront of us
			float sign = Mathf.Sign(Vector3.Dot (Camera.main.transform.forward, (worldPoint - Camera.main.transform.position).normalized));
			
			// Is our off screen? or behind us?
			if (Mathf.Abs(direction.x) >= 0.5f || Mathf.Abs(direction.y) >= 0.5f || sign == -1) {
				// Clamp to the direction to our viewport bounds (from the screen center, -0.5 <--> 0.5)
				direction.Normalize();
				if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y)) {
					if (direction.x != 0)
						direction.y = Mathf.Abs(0.5f/direction.x) * direction.y;
					
					direction.x = Mathf.Clamp(direction.x, -0.5f, 0.5f);
				} else {
					if (direction.y != 0)
						direction.x = Mathf.Abs(0.5f/direction.y) * direction.x;
					
					direction.y = Mathf.Clamp(direction.y, -0.5f, 0.5f);
				}
				
				if (sign == -1)
					direction *= -1f;
				
			}
			
			return new Vector2(0.5f + direction.x, 0.5f + direction.y);	
		}
		
		/// <summary>
		/// Returns true if the given world point is inside the viewport
		/// </summary>
		public static bool IsInViewport(Vector3 worldPoint) {
			
			if (!Camera.main)
				return false;
			
			// Get our direction in viewport space from the center of the screen
			Vector2 direction = Camera.main.WorldToViewportPoint(worldPoint)-new Vector3(0.5f,0.5f,0);
			
			// Is our target behind or infront of us
			float sign = Mathf.Sign(Vector3.Dot (Camera.main.transform.forward, (worldPoint - Camera.main.transform.position).normalized));
			
			// Is our off screen? or behind us?
			if (Mathf.Abs(direction.x) >= 0.5f || Mathf.Abs(direction.y) >= 0.5f || sign == -1) {
				return false;
			}
			
			return true;
		}
		
		public static Vector3 RandomDirection()
		{
			return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
		}
	}
}
