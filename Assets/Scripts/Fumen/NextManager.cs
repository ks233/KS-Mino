using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextManager : MonoBehaviour
{

    public Queue<int> nextQueue = new Queue<int>();
    public bool useRandomNext = true;
    public string nextSeq = "";
    int[] bag = new int[] { 1, 2, 3, 4, 5, 6, 7 };//7bag»úÖÆ

    public NextManager()
    {
        ResetNext();
    }
    // Start is called before the first frame update
    public 
        void ApplyNextSeq()
    {
        int len = nextSeq.Length;
        Mino m = new Mino();
        bool ValidNextSequence = true;
        Debug.Log(nextSeq);
        foreach (char c in nextSeq)
        {
            if (m.NameToId(c.ToString()) == -1) ValidNextSequence = false;
        };
        if (ValidNextSequence)
        {
            nextQueue.Clear();
            foreach (char c in nextSeq)
            {
                nextQueue.Enqueue(m.NameToId(c.ToString()));
            }
        }
    }

    public void SetNextSeq(string seq)
    {
        nextSeq = seq;
    }
    public void setUseRandomNext(bool b)
    {
        useRandomNext = b;
    }

    public void ResetNext()
    {
        nextQueue.Clear();
        if (useRandomNext)
        {
            bag = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            Shuffle(bag);
            nextQueue = new Queue<int>();
            updateNext();
            updateNext();
        }
        else
        {
            ApplyNextSeq();
        }
    }
    public int Dequeue()
    {

        if (nextQueue.Count <= 7 && useRandomNext) updateNext();
        if (nextQueue.Count == 0) return 0;
        return nextQueue.Dequeue();
    }
    public void updateNext()
    {

        Shuffle(bag);
        for (int i = 0; i < 7; i++)
            nextQueue.Enqueue(bag[i]);

    }
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

}
