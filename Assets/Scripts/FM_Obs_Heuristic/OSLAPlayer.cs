using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSLAPlayer : MonoBehaviour, IAI
{
    //[SerializeField] private CardSystem _cardSystem;
    [SerializeField] private DeployCardSystem _deployCardSystem;
    [SerializeField] private Observer _observer;
    [SerializeField] private ForwardModel _forwardModel;

    [SerializeField] private Transform upAttackTransform;
    [SerializeField] private Transform upDefenseTransform;
    [SerializeField] private Transform downAttackTransform;
    [SerializeField] private Transform downDefenseTransform;

    private List<Vector3> _positions = new List<Vector3>();
    private bool _thinking = false;

    public int player;

    public string id { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        id = "OSLA Player";
        _positions.Add(upAttackTransform.position);
        _positions.Add(upDefenseTransform.position);
        _positions.Add(downAttackTransform.position);
        _positions.Add(downDefenseTransform.position);
        StartCoroutine(nameof(UpdateAI));
    }

    private IEnumerator UpdateAI()
    {
        while (_observer.timeLeft >=0 && _observer.Player1KingTower.health > 0 && _observer.Player2KingTower.health >0)
        {
            //int index = think(_observer, 1f);
            if(!_thinking) StartCoroutine(nameof(ThinkCoroutine));
            yield return new WaitForSeconds(1f);
        }
    }

    public int think(Observer observer, float budget)
    {
        List<Card> availableCards = _observer.AvailableTroops(player);

        float bestScore = -1000000;
        foreach (Card troop in availableCards)
        {
            _forwardModel.Simulate(_observer, 30, Think2);
            
            float score = Heuristic.GetScore(_observer, _forwardModel.simulationObserver);
            if (score > bestScore)
            {
                bestScore = score;
            }
            
        }
        var bestAction = 1;

        return bestAction;
    }

    private int Think2()
    {
        Debug.Log("Heuristic: " + Heuristic.GetScore(_observer, _forwardModel.simulationObserver));
        return 1;
    }
    
    IEnumerator ThinkCoroutine()
    {
        _thinking = true;
        if(!_forwardModel.finished) yield break;
        Debug.Log("Pre Pensando");
        List<Card> availableCards = _observer.AvailableTroops(player);
        Debug.Log("Pensando");
        if (availableCards.Count == 0) yield break;
        
        float bestScore = -1000000;
        Card bestCard = availableCards[0];
        Vector3 bestPos = _positions[0];

        
        foreach (Card troop in availableCards)
        {
            foreach (Vector3 position in _positions)
            {
                Debug.Log("Voy a simular");
                if(player == 0) _forwardModel.SimulateInP1(_observer, 30, Build1Troop2Deploy(troop, position),()=>1);
                else _forwardModel.SimulateInP2(_observer, 30, Build1Troop2Deploy(troop, position),()=> 1);

                yield return new WaitUntil(() => _forwardModel.finished);

                float score;
                if(player == 0) { score = Heuristic.GetScore(_observer, _forwardModel.simulationObserver); }
                else { score = -Heuristic.GetScore(_observer, _forwardModel.simulationObserver); }

                if (score <= bestScore) continue;
                
                bestScore = score;
                bestCard = troop;
                bestPos = position;
            }
        }
        _deployCardSystem.DeployCard(bestCard, bestPos);
        _thinking = false;
        Debug.Log("Terminé de pensar");
    }

    private TroopsToDeploy Build1Troop2Deploy(Card troop, Vector3 position)
    {
        var card = new[] { troop };
        var time = new[] { 0.0f };
        var pos = new[] { position };
        
        return new TroopsToDeploy(card, time, pos);
    }
    
}
