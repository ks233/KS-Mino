using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Next
{
    public Queue<int> nextQueue = new Queue<int>();     //NEXT序列
    private static int[] bag = new int[] { 1, 2, 3, 4, 5, 6, 7 };      //7bag机制


    public bool useCustomNext = true;                   //是否自定义NEXT
    public string nextSeq = "";                         //自定义NEXT序列字符串



    public Next(bool _useCustomNext)
    {
        useCustomNext = _useCustomNext;
        ResetNext();
    }
    public void InsertFront(int minoId)
    {
        Queue<int> q = new Queue<int>();
        q.Enqueue(minoId);
        for (int i = 0; i < nextQueue.Count; i++)
        {
            q.Enqueue(nextQueue.Dequeue());
        }
        nextQueue = q;
    }
    public Next Clone()
    {

        Next clone = (Next)this.MemberwiseClone();
        clone.nextQueue = new Queue<int>(nextQueue);
        return clone;
    }
    public Next CloneAndDequeue()
    {
        Next next = Clone();
        next.Dequeue();
        return next;
    }
    public override string ToString()
    {
        string s = "";
        foreach(int id in nextQueue)
        {
            s += Mino.IdToName(id);
        }
        return s;
    }
    
    private void ApplyNextSeq()
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
        ApplyNextSeq();
    }

    public void ResetNext()
    {
        nextQueue.Clear();
        if (!useCustomNext)
        {
            nextQueue = new Queue<int>();
            New7Bag();
            New7Bag();
        }
        else
        {
            ApplyNextSeq();
        }
    }

    public int NextMino()
    {
        if (nextQueue.Count <= 14 && !useCustomNext) New7Bag();
        return Dequeue();
    }

    public int Dequeue()
    {
        if (nextQueue.Count == 0) return 0;
        return nextQueue.Dequeue();
    }


    public void New7Bag()
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
