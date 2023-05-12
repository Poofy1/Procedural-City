using System.Collections;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    public CityGenerator cityGenerator;
    public float speed = 10f;
    public int x = 0;
    public int y = 0;

    void Update()
    {
        Move();
    }

    void Move()
    {
        // Move the car towards the current waypoint
        transform.position = Vector3.MoveTowards(transform.position, cityGenerator.waypoints[x][y], speed * Time.deltaTime);

        // If the car reaches the waypoint, update waypointIndex to the next one
        if (Vector3.Distance(transform.position, cityGenerator.waypoints[x][y]) < 0.1f)
        {

            float rand = Random.Range(0, 4);
            switch (rand)
            {
                case 0: // Increment x if possible
                    if (x < cityGenerator.xMax) x++;
                    else x--;
                    break;
                case 1: // Decrement x if it's not at 1
                    if (x == 1) x++;
                    else x--;
                    break;
                case 2: // Increment y if possible
                    if (y < cityGenerator.yMax) y++;
                    else y--;
                    break;
                case 3: // Decrement y if it's not at 1
                    if (y == 1) y++;
                    else y--;
                    break;
            }

        }

        // Rotate the car to face the next waypoint
        Vector3 direction = cityGenerator.waypoints[x][y] - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);
    }
}