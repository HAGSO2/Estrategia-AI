using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class BinTree
{
    private State top;

    public BinTree(string name,Func<bool> cond, Func<int> funcItsf, bool n, bool p)
    {
        top = new State(name, cond, funcItsf, n, p);
    }

    public void DoStep()
    {
        State actual = top;
        while (actual != null)
        {
//            Debug.Log(actual.name);
            actual.stateFunc.Invoke();
            if (actual.condition.Invoke())
                actual = actual.positive;
            else
                actual = actual.negative;
        }
    }

    public void InsertState(string name,Func<bool> cond, Func<int> funcItsf, bool n, bool p)
    {
        State toInsert = new State(name, cond, funcItsf, n, p);
        Insert(ref top,toInsert);
    }

    private bool Insert(ref State s, State toI)
    {
        if (s == null)
        {
            s = toI;
            return s.hasNeg || s.hasPos;
        }

        if (s.hasNeg)
        {
            s.hasNeg = Insert(ref s.negative, toI);
            return s.hasNeg || s.hasPos;
        }

        if (s.hasPos)
        {
            s.hasPos = Insert(ref s.positive, toI);
            return s.hasNeg || s.hasPos;
        }

        return false;
    }

    private class State
    {
        public string name { get; }
        public State negative;
        public State positive;
        public Func<bool> condition;
        public Func<int> stateFunc;
        public bool hasNeg;
        public bool hasPos;

        public State(string name,Func<bool> cond, Func<int> funcItsf, bool n, bool p)
        {
            this.name = name;
            condition = cond;
            stateFunc = funcItsf;
            hasNeg = n;
            hasPos = p;
            negative = null;
            positive = null;
        }

        public override string ToString()
        {
            return name + ":" + hasNeg + ":" + hasPos;
        }
    }

    public override string ToString()
    {
        return SearchChilds(top);
    }

    private string SearchChilds(State actual)
    {
        string msg = actual.ToString();
        msg += "\t";
        if(actual.negative != null)
            msg += "Negative: " + SearchChilds(actual.negative);
        msg += "\t";
        if(actual.positive != null)
            msg += "Positive: " + SearchChilds(actual.positive);
        msg += "\n";

        return msg;
    }
}


