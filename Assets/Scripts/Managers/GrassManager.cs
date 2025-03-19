using UnityEngine;

public class GrassManager : MonoBehaviour
{
    private int nbGrass;
    public GameObject grass;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nbGrass = Random.Range(3, 6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopGrass(Transform coordT)
    {
        for (int a = 0; a<nbGrass; a++)
        {
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(-1f, 1f);
            Instantiate(grass, coordT.position + new Vector3(randomX, randomY), Quaternion.identity);
        }
    }
}