using UnityEngine;

public class CarAI : MonoBehaviour
{
    public CityGenerator cityGenerator;
    public int maxParticles = 1000;
    public float speed = 10f;

    private ParticleSystem particleSystem;
    
    private int xMax;
    private int yMax;

    private int[] x;
    private int[] y;
    private Vector3[] target;
    private int xOffset;
    private int yOffset;

    private ParticleSystem.Particle[] particles;

    void Start()
    {
        xMax = cityGenerator.cityWidth - 1;
        yMax = cityGenerator.cityLength - 1;

        x = new int[maxParticles];
        y = new int[maxParticles];
        target = new Vector3[maxParticles];
        
        particleSystem = GetComponent<ParticleSystem>();
        
        var main = particleSystem.main;
        main.loop = false;
        main.startSpeed = 0;
        main.startLifetime = Mathf.Infinity;
        
        particles = new ParticleSystem.Particle[maxParticles];
        particleSystem.maxParticles = maxParticles;

        // Emit particles manually
        for (int i = 0; i < maxParticles; i++)
        {
            x[i] = Random.Range(1, xMax);
            y[i] = Random.Range(1, yMax);
            SetTarget(i);
            
            particleSystem.Emit(new ParticleSystem.EmitParams()
            {
                position = target[i],
                velocity = Vector3.zero,
                startLifetime = Mathf.Infinity,
            }, 1);

            
        }
    }

    void Update()
    {
        MoveParticles();
    }

    void MoveParticles()
    {
        int particleCount = particleSystem.GetParticles(particles);
        
        for (int i = 0; i < particleCount; i++)
        {
            // Move the particle towards the waypoint
            Vector3 direction = (target[i] - particles[i].position).normalized;
            particles[i].velocity = direction * speed;

            // If the particle reaches the waypoint, select a new waypoint
            if (Vector3.Distance(particles[i].position, target[i]) < 0.1f)
            {
                SetTarget(i);
            }
        }

        // Apply the particle changes to the particle system
        particleSystem.SetParticles(particles, particleCount);
    }

    private void SetTarget(int i)
    {
        int rand = Random.Range(0, 4);
        if(rand == 0 || rand == 1)
        {
            if (rand == 0 && x[i] < xMax || rand == 1 && x[i] == 1)
            {
                x[i]++;
                xOffset = 2;
                yOffset = -2;
            }
            else
            {
                x[i]--;
                xOffset = -2;
                yOffset = 2;
            }
        }
        else
        {
            if (rand == 2 && y[i] < yMax || rand == 3 && y[i] == 1)
            {
                y[i]++;
                xOffset = 2;
                yOffset = 2;
            }
            else
            {
                y[i]--;
                xOffset = -2;
                yOffset = -2;
            }
        }
                
        //Set new target
        target[i] = cityGenerator.waypoints[x[i]][y[i]] + new Vector3(xOffset, 0, yOffset);
    }
}
