using UnityEditor;
using UnityEngine;

public class ExplosionDeath : MonoBehaviour
{
    public int cubePerAxis = 8;
    public float delay = 1f;
    public float force = 0;
    public float radius = 2f; 
    // Update is called once per frame
    void Start()
    {
        Invoke("Main", delay);
    }

    void Main()
    {
        for (int x = 0; x < cubePerAxis; x++)
        {
            for (int y = 0; y < cubePerAxis; y++)
            {
                for (int z = 0; z < cubePerAxis; z++)
                {
                    CreateCube(new Vector3(x, y, z));
                }
            }
        }
        Destroy(gameObject);
    }

    void CreateCube(Vector3 coords)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        Renderer rd = cube.GetComponent<Renderer>();
        rd.material = GetComponent<Renderer>().material;

        cube.transform.localScale = transform.localScale / cubePerAxis;

        Vector3 firstCube =  transform.position - transform.localScale / 2 + cube.transform.localScale / 2;
        cube.transform.position = firstCube + Vector3.Scale(coords, cube.transform.localScale);

        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.AddExplosionForce(force, transform.position, radius);
    }
}
