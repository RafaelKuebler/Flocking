using UnityEngine;

public class FishInstantiator : MonoBehaviour
{
    [SerializeField]
    private GameObject fishPrefab;

    [SerializeField]
    private int number;

    private void Start()
    {
        for (int i = 0; i < number; i++)
        {
            var positionX = Random.Range(-8, 8);
            var positionY = Random.Range(-5, 5);
            Instantiate(fishPrefab, new Vector3(positionX, positionY, 0), Quaternion.identity);
        }
    }
}