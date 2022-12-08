using System;
using System.Collections;
using System.Collections.Generic;
using Unity;

public class TroopConfig
{
    enum myTroop
    {
        Archer = 2,
        Barbarian = 4,
        Giant = 5,
        Goblin = 1,
        Knight = 3,
        MiniPekka = 4
        /*AtqWall = 6,
        Barbarity = 7,
        SlowHit = 8,
        Support = 9,
        BigPlay = 10,*/
    }
    enum spawnPosition
    {
        For = 0,
        Back = 1,
        Any = 2
    }
    struct Plays
    {
        private myTroop[] _implicated;
        private spawnPosition[] _sided;

        public Plays(myTroop[] t, spawnPosition[] s)
        {
            _implicated = t;
            _sided = s;
        }
    }
    struct PlaysWeight
    {
        private Plays _required;
        private int _elixir;
        private float _hValue;
    }

    private Plays[] _disponible;
    private PlaysWeight[] _info;
    public TroopConfig(string[] cards)
    {
        List<Plays> _pls = new List<Plays>();
        int[] already = new int[5];   
        foreach (string card in cards)
        {
            switch (card)
            {
                case "Archer":
                    //Archer
                    _pls.Add(new Plays(new [] { myTroop.Archer },new [] { spawnPosition.Any }));
                    already[0]++;
                    break;
                case "Barbarian":
                    //Barbarian
                    _pls.Add(new Plays(new [] { myTroop.Barbarian },new [] { spawnPosition.Any }));
                    already[1]++;
                    break;
                case "Giant":
                    //Giant
                    _pls.Add(new Plays(new [] { myTroop.Giant },new [] { spawnPosition.For }));
                    already[2]++;
                    break;
                case "Goblin":
                    //Goblin
                    _pls.Add(new Plays(new [] { myTroop.Goblin },new [] { spawnPosition.Back }));
                    already[3]++;
                    break;
                case "Knight":
                    _pls.Add(new Plays(new [] { myTroop.Knight },new [] { spawnPosition.For }));
                    already[4]++;
                    break;
                case "Mini-Pekka":
                    _pls.Add(new Plays(new [] { myTroop.MiniPekka },new [] { spawnPosition.For }));
                    break;
            }
            if (already[0] != 0 && already[1] != 0)
            {
                _pls.Add(new Plays(new [] { myTroop.Archer ,myTroop.Giant},new [] { spawnPosition.Back ,spawnPosition.For}));
                already[0]--;
                already[1]--;
            }

            if (already[1] != 0 && already[3] != 0)
            {
                _pls.Add(new Plays(new [] { myTroop.Goblin ,myTroop.Giant},new [] { spawnPosition.Back ,spawnPosition.For}));
                already[3]--;
                already[1]--;
            }

            if (already[2] >= 2)
            {
                _pls.Add(new Plays(new [] { myTroop.Barbarian ,myTroop.Barbarian},new [] { spawnPosition.Back ,spawnPosition.Back}));
                already[2] -= 2;
            }

            if (already[4] >= 2)
            {
                _pls.Add(new Plays(new [] { myTroop.Knight ,myTroop.Knight},new [] { spawnPosition.For ,spawnPosition.For}));
                already[4] -= 2;
            }
        }
    }

    public void DoPlays(Observer obs,ForwardModel fm)
    {
    }
}