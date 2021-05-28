using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextManager
{

    public Queue<int> nextQueue = new Queue<int>();     //NEXT序列
    public bool useCustomNext = true;                   //是否自定义NEXT
    public string nextSeq = "";                         //NEXT序列字符串
    int[] bag = new int[] { 1, 2, 3, 4, 5, 6, 7 };      //7bag机制

    public NextManager(bool _useCustomNext)
    {
        useCustomNext = _useCustomNext;
        ResetNext();
    }


    public void ApplyNextSeq()
    {
        int len = nextSeq.Length;
        Mino m = new Mino();
        bool ValidNextSequence = true;
        Debug.Log(nextSeq);
        foreach (char c in nextSeq)
        {
            if (Mino.NameToId(c.ToString()) == -1) ValidNextSequence = false;
        };
        if (ValidNextSequence)
        {
            nextQueue.Clear();
            foreach (char c in nextSeq)
            {
                nextQueue.Enqueue(m.GetIdInt());
            }
        }
    }



    public void SetNextSeq(string seq)
    {
        nextSeq = seq;
    }

    public void SetUseCustomNext(bool b)
    {
        useCustomNext = b;
    }

    public void ResetNext()
    {
        nextQueue.Clear();
        if (!useCustomNext)
        {
            bag = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            Shuffle(bag);
            nextQueue = new Queue<int>();
            UpdateNext();
            UpdateNext();
        }
        else
        {
            ApplyNextSeq();
        }
    }


    public int Dequeue()
    {

        //return Random.Range(1, 8);

        if (nextQueue.Count <= 7 && !useCustomNext) UpdateNext();
        if (nextQueue.Count == 0) return 0;
        return nextQueue.Dequeue();
    }


    public void UpdateNext()
    {

        Shuffle(bag);
        for (int i = 0; i < 7; i++)
            nextQueue.Enqueue(bag[i]);

    }
    public static void Shuffle(int[] deck)//打乱包中的块序
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
