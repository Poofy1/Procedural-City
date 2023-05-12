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
    public int roadWidth = 1;
    public float heightFrequency = 0.01f;
    public float heightFrequency_small = 0.25f;

    [Header("Building Config")] 
    public int buildingSize = 5;
    public int buildingMax = 75;
    public int buildingMin = 5;
    public float skyScraperThreshold = 4;
    private List<CombineInstance> buildingMeshFilters_small;
    private List<CombineInstance> buildingMeshFilters_tall;
    private List<CombineInstance> roofMeshFilters;

    [Header("Car Config")] 
    public GameObject cars;

    [Header("Other")]
    public Vector3[][] waypoints;

    private void Start()
    {
        GenerateCity();
        cars.SetActive(true);
    }


    private void GenerateCity()
    {
        
        buildingMeshFilters_small = new List<CombineInstance>();
        buildingMeshFilters_tall = new List<CombineInstance>();
        roofMeshFilters = new List<CombineInstance>();
        waypoints = new Vector3[cityWidth][];
        CreateRoads();

        for (int x = 0; x < cityWidth; x++)
        {
            waypoints[x] = new Vector3[cityLength];
            for (int z = 0; z < cityLength; z++)
            {
                
                Vector3 roadPosition = new Vector3(x * (blockWidth * buildingSize) + (x * roadWidth), 1, z * (blockLength * buildingSize) + (z * roadWidth));
                waypoints[x][z] = roadPosition;
                    
                for (int block_x = 0; block_x < blockWidth; block_x++)
                {
                    for (int block_y = 0; block_y < blockLength; block_y++)
                    {
                        Vector3 buildingPosition = new Vector3(roadPosition.x + (block_x * buildingSize) + (roadWidth/2), 0, roadPosition.z + (block_y * buildingSize) + (roadWidth/2));
                        
                        float perlinValue = Mathf.PerlinNoise(buildingPosition.x * heightFrequency, buildingPosition.z * heightFrequency);
                        float perlinValue2 = Mathf.PerlinNoise(buildingPosition.x * heightFrequency_small, buildingPosition.z * heightFrequency_small);
                        perlinValue = Mathf.Pow(perlinValue, 6) + Mathf.Pow(perlinValue2, 6);
                        
                        //Create Building Walls
                        int buildingHeight = Mathf.FloorToInt(Mathf.Lerp(buildingMin, buildingMax, perlinValue));
                        CreateBuilding(buildingSize - 1, buildingSize - 1, buildingHeight, buildingPosition);
                        
                        //Create Building Roofs
                        Vector3 roofPosition = new Vector3(buildingPosition.x, buildingHeight, buildingPosition.z);
                        CreateRoof(buildingSize - 1, buildingSize - 1, buildingHeight, roofPosition);
                        
                        //Create Antennas
                        if (buildingHeight >= skyScraperThreshold && Random.value < 0.1f)
                        {
                            Vector3 antennaPosition = new Vector3(Random.Range(1, buildingSize-1), 0, Random.Range(1, buildingSize-1));
                            GameObject antennaInstance = Instantiate(antenna, roofPosition + antennaPosition, Quaternion.identity);
                            antennaInstance.transform.SetParent(transform);
                        }
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
            new Vector3(cityWidth * (blockWidth * buildingSize + roadWidth), 0, 0),
            new Vector3(cityWidth * (blockWidth * buildingSize + roadWidth), 0, cityLength * (blockLength * buildingSize + roadWidth)),
            new Vector3(0, 0, cityLength * (blockLength * buildingSize + roadWidth))
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
    
    
    private void CreateBuilding(float width, float length, float height, Vector3 pos)
    {
        Mesh buildingMesh = new Mesh();

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

        buildingMesh.vertices = vertices;
        buildingMesh.uv = uv;
        buildingMesh.triangles = buildingTriangles;
        buildingMesh.RecalculateNormals();
        
        CombineInstance combineInstance = new CombineInstance();
        combineInstance.mesh = buildingMesh;
        
        combineInstance.transform = Matrix4x4.Translate(pos);

        // Apply the new material if the height is below the threshold
        if (height < skyScraperThreshold)
        {
            buildingMeshFilters_small.Add(combineInstance);
        }
        else
        {
            buildingMeshFilters_tall.Add(combineInstance);
        }
    }
    
    private void CreateRoof(float width, float length, float height, Vector3 pos)
    {
        Mesh roofMesh = new Mesh();

        Vector3[] roofVertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(width, 0, length),
            new Vector3(0, 0, length)
        };
        
        Vector2[] roofUV = new Vector2[4];

        roofUV[0] = new Vector2(0, 0);
        roofUV[1] = new Vector2(1, 0);
        roofUV[2] = new Vector2(1, 1);
        roofUV[3] = new Vector2(0, 1);

        int[] roofTriangles = new int[]
        {
            0, 2, 1, // Top
            0, 3, 2
        };
        
        roofMesh.vertices = roofVertices;
        roofMesh.uv = roofUV;
        roofMesh.triangles = roofTriangles;
        roofMesh.RecalculateNormals();
        
        CombineInstance combineInstance = new CombineInstance();
        combineInstance.mesh = roofMesh;
        
        combineInstance.transform = Matrix4x4.Translate(pos);
        roofMeshFilters.Add(combineInstance);
    }
    
    
    
    void MergeMeshes(List<CombineInstance> combineInstances, Material material)
    {
        // Create new mesh on children meshes
        CombineInstance[] combine = combineInstances.ToArray();

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
    
    
    
    
}