using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BinaryNode : MonoBehaviour
{

    private string Title;

    private BinaryNode leftChild;
    private BinaryNode rightChild;
    private Func<bool> Test;

    public BinaryNode(string title, BinaryNode left, BinaryNode right, Func<bool> f)
    {
        Title = title;
        leftChild = left;
        rightChild = right;
        Test = f;
    }

    public BinaryNode(string title, Func<bool> f)
    {
        Title = title;
        leftChild = null;
        rightChild = null;
        Test = f;
    }

    public BinaryNode LeftChild()
    {
        return leftChild;
    }

    public BinaryNode RightChild()
    {
        return rightChild;
    }

    public void Evaluate()
    {
        if (leftChild == null)
            return;

        bool result = Test();

        if (result) leftChild.Evaluate();
        else rightChild.Evaluate();
    }
}
