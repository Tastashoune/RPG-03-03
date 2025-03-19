using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int mapWidth = 10;
    public int mapHeight = 10;
    public LayerMask obstacleLayer;

    private int nbCoins = 10;
    public GameObject coin;
    void Start()
    {
        for(int a=1; a<= nbCoins; a++)
            Instantiate(coin, GetRandomCoordinate(), Quaternion.identity);
    }

    public Vector2 GetRandomCoordinate()
    {
        Vector2 randomCoordinate;

        do
        {
            float x = Random.Range(-mapWidth, mapWidth);
            float y = Random.Range(-mapHeight, mapHeight);
            randomCoordinate = new Vector2(x, y);
        }
        while (IsObstacleAt(randomCoordinate));

        return randomCoordinate;
    }

    private bool IsObstacleAt(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position, obstacleLayer);
        return hit != null;
    }
}