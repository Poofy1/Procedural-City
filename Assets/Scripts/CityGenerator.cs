using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject antenna;

    [Header("Materials")] 
    public Material groundMaterial;
    public Material buildingMaterial;
    public Material lowHeightBuildingMaterial;
    public Material roofMaterial;

    [Header("City Config")] 
    public int cityWidth = 10;
    public int cityLength = 10;
    public int blockWidth = 2;
    public int blockLength = 4;
    public float heightFrequency = 0.01f;
    public float heightFrequency_small = 0.25f;

    [Header("Building Config")] 
    public int buildingSize = 5;
    public int buildingMax = 75;
    public int buildingMin = 5;
    public float skyScraperThreshold = 4;
    private List<MeshFilter> buildingMeshFilters_small;
    private List<MeshFilter> buildingMeshFilters_tall;
    private List<MeshFilter> roofMeshFilters;

    [Header("Car Config")] 
    public GameObject car;
    public int carAmount;
    [NonSerialized] public int xMax = 0;
    [NonSerialized] public int yMax = 0;
    
    [Header("Other")]
    public Vector3[][] waypoints;

    private void Start()
    {
        GenerateCity();
        GenerateCars();
    }


    private void GenerateCity()
    {
        
        buildingMeshFilters_small = new List<MeshFilter>();
        buildingMeshFilters_tall = new List<MeshFilter>();
        roofMeshFilters = new List<MeshFilter>();
        waypoints = new Vector3[cityWidth][];
        CreateRoads();

        for (int x = 0; x < cityWidth; x++)
        {
            waypoints[x] = new Vector3[cityLength];
            for (int z = 0; z < cityLength; z++)
            {
                
                Vector3 roadPosition = new Vector3(x * (blockWidth * buildingSize) + (x * 3), 1, z * (blockLength * buildingSize) + (z * 3));
                waypoints[x][z] = roadPosition;
                    
                for (int block_x = 0; block_x < blockWidth; block_x++)
                {
                    for (int block_y = 0; block_y < blockLength; block_y++)
                    {
                        Vector3 buildingPosition = new Vector3(roadPosition.x + (block_x * buildingSize) + 2f, 0,
                            roadPosition.z + (block_y * buildingSize) + 2f);
                        float perlinValue = Mathf.PerlinNoise(buildingPosition.x * heightFrequency,
                            buildingPosition.z * heightFrequency);
                        float perlinValue2 = Mathf.PerlinNoise(buildingPosition.x * heightFrequency_small,
                            buildingPosition.z * heightFrequency_small);
                        perlinValue = Mathf.Pow(perlinValue, 6) + Mathf.Pow(perlinValue2, 6);
                        
                        int buildingHeight = Mathf.FloorToInt(Mathf.Lerp(buildingMin, buildingMax, perlinValue));
                        
                        GameObject building = CreateBuilding(buildingSize - 1, buildingSize - 1, buildingHeight);
                        building.transform.position = buildingPosition;
                        building.transform.SetParent(transform);
                        
                        GameObject roof = CreateRoof(buildingSize - 1, buildingSize - 1, buildingHeight);
                        roof.transform.position = new Vector3(buildingPosition.x, buildingHeight, buildingPosition.z);
                        roof.transform.SetParent(transform);
                    }
                }
            }
        }

        MergeMeshes(buildingMeshFilters_small, lowHeightBuildingMaterial);
        MergeMeshes(buildingMeshFilters_tall, buildingMaterial);
        MergeMeshes(roofMeshFilters, roofMaterial);
    }

    private void CreateRoads()
    {
        // Create a new game object to hold the ground
        GameObject ground = new GameObject("Ground");
        ground.transform.position = Vector3.zero;
        ground.transform.SetParent(transform);

        // Add MeshRenderer and MeshFilter components
        MeshFilter groundFilter = ground.AddComponent<MeshFilter>();
        MeshRenderer groundRenderer = ground.AddComponent<MeshRenderer>();

        // Create a new mesh
        Mesh groundMesh = new Mesh();

        // Define vertices for a plane of size cityWidth * cityLength
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(cityWidth * (blockWidth * buildingSize + 3), 0, 0),
            new Vector3(cityWidth * (blockWidth * buildingSize + 3), 0, cityLength * (blockLength * buildingSize + 3)),
            new Vector3(0, 0, cityLength * (blockLength * buildingSize + 3))
        };

        // Define UVs for the vertices
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(cityWidth, 0),
            new Vector2(cityWidth, cityLength),
            new Vector2(0, cityLength)
        };

        // Define the two triangles that make up the square
        int[] triangles = new int[6]
        {
            0, 2, 1,
            0, 3, 2
        };

        // Assign the vertices, UVs, and triangles to the mesh
        groundMesh.vertices = vertices;
        groundMesh.uv = uv;
        groundMesh.triangles = triangles;

        // Assign the mesh to the MeshFilter component
        groundFilter.mesh = groundMesh;

        // Assign a material to the MeshRenderer component
        groundRenderer.material = groundMaterial;
    }

    
    
    private GameObject CreateBuilding(float width, float length, float height)
    {
        GameObject building = new GameObject("Building");
        MeshFilter buildingMeshFilter = building.AddComponent<MeshFilter>();
        MeshRenderer buildingMeshRenderer = building.AddComponent<MeshRenderer>();

        Mesh buildingMesh = new Mesh();
        buildingMeshFilter.mesh = buildingMesh;

        Vector3[] vertices = new Vector3[16]
        {
            new Vector3(0, 0, 0), // Front
            new Vector3(width, 0, 0),
            new Vector3(width, height, 0),
            new Vector3(0, height, 0),

            new Vector3(width, 0, 0), // Right
            new Vector3(width, 0, length),
            new Vector3(width, height, length),
            new Vector3(width, height, 0),

            new Vector3(width, 0, length), // Back
            new Vector3(0, 0, length),
            new Vector3(0, height, length),
            new Vector3(width, height, length),

            new Vector3(0, 0, length), // Left
            new Vector3(0, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(0, height, length)
        };

        buildingMesh.vertices = vertices;
        Vector2[] uv = new Vector2[16];

        // Front face
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(1, height);
        uv[3] = new Vector2(0, height);

        // Right face
        uv[4] = new Vector2(0, 0);
        uv[5] = new Vector2(1, 0);
        uv[6] = new Vector2(1, height);
        uv[7] = new Vector2(0, height);

        // Back face
        uv[8] = new Vector2(0, 0);
        uv[9] = new Vector2(1, 0);
        uv[10] = new Vector2(1, height);
        uv[11] = new Vector2(0, height);

        // Left face
        uv[12] = new Vector2(0, 0);
        uv[13] = new Vector2(1, 0);
        uv[14] = new Vector2(1, height);
        uv[15] = new Vector2(0, height);


        for (int i = 0; i < uv.Length; i++)
        {
            if (i < 4 || (i >= 8 && i < 12))
            {
                uv[i].x *= width;
            }
            else
            {
                uv[i].x *= length;
            }
        }

        buildingMesh.uv = uv;

        int[] buildingTriangles = new int[]
        {
            3, 1, 0, // Front
            3, 2, 1,
            4, 7, 5, // Right
            5, 7, 6,
            11, 9, 8, // Back
            11, 10, 9,
            12, 15, 13, // Left
            13, 15, 14
        };

        buildingMesh.triangles = buildingTriangles;
        buildingMesh.RecalculateNormals();

        // Apply the new material if the height is below the threshold
        if (height < skyScraperThreshold)
        {
            buildingMeshFilters_small.Add(buildingMeshFilter);
        }
        else
        {
            buildingMeshFilters_tall.Add(buildingMeshFilter);
        }

        return building;
    }
    
    private GameObject CreateRoof(float width, float length, float height)
    {

        // Create the roof as a separate GameObject
        GameObject roof = new GameObject("Roof");
        MeshFilter roofMeshFilter = roof.AddComponent<MeshFilter>();
        roofMeshFilters.Add(roofMeshFilter);
        MeshRenderer roofMeshRenderer = roof.AddComponent<MeshRenderer>();

        Mesh roofMesh = new Mesh();
        roofMeshFilter.mesh = roofMesh;

        Vector3[] roofVertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(width, 0, length),
            new Vector3(0, 0, length)
        };
        
        roofMesh.vertices = roofVertices;

        Vector2[] roofUV = new Vector2[4];
        
        // Roof UV mapping
        roofUV[0] = new Vector2(0, 0);
        roofUV[1] = new Vector2(1, 0);
        roofUV[2] = new Vector2(1, 1);
        roofUV[3] = new Vector2(0, 1);
        
        roofMesh.uv = roofUV;

        int[] roofTriangles = new int[]
        {
            0, 2, 1, // Top
            0, 3, 2
        };
        
        roofMesh.triangles = roofTriangles;
        roofMesh.RecalculateNormals();
        
        roofMeshRenderer.material = roofMaterial;
        

        // Apply the new material if the height is below the threshold
        if (height >= skyScraperThreshold && Random.value < 0.1f)
        {
            Vector3 antennaPosition = new Vector3(Random.Range(1, buildingSize-1), 0, Random.Range(1, buildingSize-1));
            GameObject antennaInstance = Instantiate(antenna, roof.transform.TransformPoint(antennaPosition), Quaternion.identity);
            antennaInstance.transform.SetParent(roof.transform);
        }

        return roof;
    }
    
    
    
    void MergeMeshes(List<MeshFilter> meshFilters, Material material)
    {
        // Create new mesh on children meshes
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];

        int i = 0;
        while (i < meshFilters.Count)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        // Create a new game object
        GameObject combined = new GameObject("Combined Mesh");
        combined.transform.SetParent(transform);

        // Add MeshFilter and MeshRenderer components
        MeshFilter meshFilter = combined.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = combined.AddComponent<MeshRenderer>();
    
        // Assign the combined meshes to the new MeshFilter component
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.mesh.CombineMeshes(combine);
        combined.SetActive(true);
    
        // Assign a material to the MeshRenderer component
        meshRenderer.material = material; 
    }
    
    
    
    public void GenerateCars()
    {
        GameObject carHolder = new GameObject("Cars");
        carHolder.transform.SetParent(transform);

        xMax = cityWidth-1;
        yMax = cityLength-1;

        for (int i = 0; i < carAmount; i++)
        {
            int x = Random.Range(1, xMax);
            int y = Random.Range(1, yMax);
            Vector3 carPosition = waypoints[x][y];
            GameObject carInstance = Instantiate(car, carPosition, Quaternion.identity);
            carInstance.transform.SetParent(carHolder.transform);
            CarAI carAI = carInstance.GetComponent<CarAI>();
            carAI.cityGenerator = gameObject.GetComponent<CityGenerator>();
            carAI.x = x;
            carAI.y = y;
        }
    }
    
}