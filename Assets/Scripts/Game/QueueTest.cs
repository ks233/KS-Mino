using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class QueueTest : MonoBehaviour
{


    public int[] bag = new int[] { 1, 2, 3, 4, 5, 6, 7 };//7bag»úÖÆ

    public Queue<int> nextQueue = new Queue<int>();

    void Shuffle(int[] deck)
    {
        for (int i = 0; i < deck.Length; i++)
        {
            int temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(0, deck.Length);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Shuffle(bag);
        for (int i = 0; i < 7; i++)
            nextQueue.Enqueue(bag[i]);
        int count = nextQueue.Count;
        for(int i = 0;i<count;i++)
            Debug.Log(nextQueue.Dequeue());


        Debug.Log("a");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
