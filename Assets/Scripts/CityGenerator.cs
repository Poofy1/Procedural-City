using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    public GameObject roadPrefab;
    public Material buildingMaterial;
    public int cityWidth = 10;
    public int cityLength = 10;
    
    public int buildingSize = 5;
    public int buildingMax = 10;
    public int buildingMin = 2;
    
    public int blockWidth = 5;
    public int blockLength = 5;
    public int roadWidth = 5;

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
                        int randomBuildingHeight = Mathf.FloorToInt(Mathf.Lerp(buildingMin, buildingMax, Mathf.Pow(Random.value, 5)));
                        Vector3 buildingPosition = new Vector3(roadPosition.x + (block_x * buildingSize) + 2f, 0, roadPosition.z + (block_y * buildingSize) + 2f);
                        GameObject building = CreateBuilding(buildingSize - 1,buildingSize - 1, randomBuildingHeight);
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

        Vector3[] vertices = new Vector3[8]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(width, 0, length),
            new Vector3(0, 0, length),
            new Vector3(0, height, 0),
            new Vector3(width, height, 0),
            new Vector3(width, height, length),
            new Vector3(0, height, length)
        };

        mesh.vertices = vertices;

        int[] triangles = new int[]
        {
            0, 1, 2, // Bottom
            0, 2, 3,
            4, 6, 5, // Top
            4, 7, 6,
            0, 5, 1, // Front
            0, 4, 5,
            1, 6, 2, // Right
            1, 5, 6,
            2, 7, 3, // Back
            2, 6, 7,
            3, 4, 0, // Left
            3, 7, 4
        };

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshRenderer.material = buildingMaterial;

        return building;
    }
}
