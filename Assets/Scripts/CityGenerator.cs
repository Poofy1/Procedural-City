using System.Linq;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    public GameObject roadPrefab;
    public Material buildingMaterial;
    public Material lowHeightBuildingMaterial; // Add new material for low height buildings
    public Material roofMaterial;
    public int cityWidth = 10;
    public int cityLength = 10;

    public int buildingSize = 5;
    public int buildingMax = 10;
    public int buildingMin = 2;

    public float heightFrequency = 0.1f;

    public int blockWidth = 5;
    public int blockLength = 5;

    public float roofTextureScale = 1;
    public float heightThreshold = 4; // Add new height threshold field

    private void Start()
    {
        GenerateCity();
    }

    private void GenerateCity()
    {
        for (int x = 0; x < cityWidth; x++)
        {
            for (int z = 0; z < cityLength; z++)
            {
                Vector3 roadPosition = new Vector3(x * (blockWidth * buildingSize) + (x * 3), 0, z * (blockLength * buildingSize) + (z * 3));
                GameObject roadInstance = Instantiate(roadPrefab, roadPosition, Quaternion.identity);
                roadInstance.transform.SetParent(transform);

                for (int block_x = 0; block_x < blockWidth; block_x++)
                {
                    for (int block_y = 0; block_y < blockLength; block_y++)
                    {
                        Vector3 buildingPosition = new Vector3(roadPosition.x + (block_x * buildingSize) + 2f, 0, roadPosition.z + (block_y * buildingSize) + 2f);
                        float perlinValue = Mathf.PerlinNoise(buildingPosition.x * heightFrequency, buildingPosition.z * heightFrequency);
                        perlinValue = Mathf.Pow(perlinValue, 4);
                        int buildingHeight = Mathf.FloorToInt(Mathf.Lerp(buildingMin, buildingMax, perlinValue));
                        GameObject building = CreateBuilding(buildingSize - 1, buildingSize - 1, buildingHeight);
                        building.transform.position = buildingPosition;
                        building.transform.SetParent(transform);
                    }
                }
            }
        }
    }
    
    private GameObject CreateBuilding(float width, float length, float height)
    {
        GameObject building = new GameObject("Building");
        MeshFilter meshFilter = building.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = building.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[16]
        {
            new Vector3(0, 0, 0),         // Front
            new Vector3(width, 0, 0),
            new Vector3(width, height, 0),
            new Vector3(0, height, 0),

            new Vector3(width, 0, 0),     // Right
            new Vector3(width, 0, length),
            new Vector3(width, height, length),
            new Vector3(width, height, 0),

            new Vector3(width, 0, length), // Back
            new Vector3(0, 0, length),
            new Vector3(0, height, length),
            new Vector3(width, height, length),

            new Vector3(0, 0, length),    // Left
            new Vector3(0, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(0, height, length)
        };
        
        Vector3[] roofVertices = new Vector3[4]
        {
            new Vector3(0, height, 0),
            new Vector3(width, height, 0),
            new Vector3(width, height, length),
            new Vector3(0, height, length)
        };

        mesh.vertices = vertices.Concat(roofVertices).ToArray();
        
        Vector2[] uv = new Vector2[20];

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
        
        // Roof UV mapping
        uv[16] = new Vector2(0, 0);
        uv[17] = new Vector2(roofTextureScale, 0);
        uv[18] = new Vector2(roofTextureScale, roofTextureScale);
        uv[19] = new Vector2(0, roofTextureScale);
        
        for (int i = 0; i < uv.Length; i++) {
            if (i < 4 || (i >= 8 && i < 12)) {
                uv[i].x *= width;
            } else {
                uv[i].x *= length;
            }
        }

        mesh.uv = uv;
        

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

        int[] roofTriangles = new int[]
        {
            16, 18, 17, // Top
            16, 19, 18
        };

        mesh.subMeshCount = 2;
        mesh.SetTriangles(buildingTriangles, 0);
        mesh.SetTriangles(roofTriangles, 1);

        mesh.RecalculateNormals();

        // Apply the new material if the height is below the threshold
        if (height < heightThreshold)
        {
            meshRenderer.materials = new Material[] { lowHeightBuildingMaterial, roofMaterial };
        }
        else
        {
            meshRenderer.materials = new Material[] { buildingMaterial, roofMaterial };
        }

        return building;
    }
}