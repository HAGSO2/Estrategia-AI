using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class BinTree
{
    private State top;
    private State actual;

    public BinTree(string name,Func<bool> cond, Func<int> funcItsf, bool n, bool p)
    {
        top = new State(name, cond, funcItsf, n, p);
        actual = top;
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

    /*private bool Insert(ref State s,State toI)
    {
        if (s.negative == null || s.positive == null)
        {
            if (s.negative == null && s.hasNeg)
            {
                s.negative = toI;
                s.hasNeg = toI.hasNeg || toI.hasPos;
                return toI.hasNeg || toI.hasPos;
            }

            if (s.positive == null && s.hasPos)
            {
                s.positive = toI;
                s.hasPos = toI.hasNeg || toI.hasPos;
                return toI.hasNeg || toI.hasPos;
            }

            return false;
        }

        bool inNeg = Insert(ref s.negative, toI);
        bool inPos = false;
        if (!inNeg) inPos = Insert(ref s.positive, toI);

        return false;
    }*/
    
    private class State
    {
        private string name;
        public State negative;
        public State positive;
        private Func<bool> _condition;
        private Func<int> _stateFunc;
        public bool hasNeg;
        public bool hasPos;

        public State(string name,Func<bool> cond, Func<int> funcItsf, bool n, bool p)
        {
            this.name = name;
            _condition = cond;
            _stateFunc = funcItsf;
            hasNeg = n;
            hasPos = p;
            negative = null;
            positive = null;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public override string ToString()
    {
        return SearchChilds(top);
    }

    private string SearchChilds(State actual)
    {
        string msg = actual.ToString();
        if(actual.negative != null)
            msg += "Negative: " + SearchChilds(actual.negative);
        if(actual.positive != null)
            msg += "Positive: " + SearchChilds(actual.positive);

        return msg;
    }
}


