using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Random=UnityEngine.Random;
using UnityEngine;

struct SituationData {
    // All the data of a situation and its reaction
    public string troop; // The troop to deploy in the situation
    public string zone;  // The zone to deploy the situation (up, down or middle)
    public double potentialDPSTotalUp; // Total potential dps of all the enemies in the up line
    public double potentialDPSTotalDown; // Total potential dps of all the enemies in the down line
    public double potentialHealthTotalUp; // Total potential health of all the enemies in the up line
    public double potentialHealthTotalDown; // Total potential dps of all the enemies in the down line
    public double DistanceToTowerOwnUp; // Mean of the distance to the tower of all the enemies in the up line
    public double DistanceToTowerOwnDown; // Mean of the distance to the tower of all the enemies in the down line
       
    //Constructor (not necessary, but helpful)
    public SituationData(string troop, string zone,double potentialDPSTotalUp,double potentialDPSTotalDown,double potentialHealthTotalUp,double potentialHealthTotalDown,double DistanceToTowerOwnUp,double DistanceToTowerOwnDown) {
        this.troop = troop;
        this.zone = zone;
        this.potentialDPSTotalUp = potentialDPSTotalUp;
        this.potentialDPSTotalDown = potentialDPSTotalDown;
        this.potentialHealthTotalUp = potentialHealthTotalUp;
        this.potentialHealthTotalDown = potentialHealthTotalDown;
        this.DistanceToTowerOwnUp = DistanceToTowerOwnUp;
        this.DistanceToTowerOwnDown = DistanceToTowerOwnDown;
    }
};

struct GaussianData {
    // All the data of a situation and its reaction
    public double mean;
    public double standarDeviation;
    public double likelihood;
       
    //Constructor (not necessary, but helpful)
    public GaussianData(double mean,double standarDeviation, double likelihood) {
        this.mean = mean;
        this.standarDeviation = standarDeviation;
        this.likelihood = likelihood;
    }
};

struct TroopGaussianData {
    // All the data of a situation and its reaction
    public GaussianData gd_PotentialDPSTotalUp;
    public GaussianData gd_PotentialDPSTotalDown;
    public GaussianData gd_PotentialHealthTotalUp;
    public GaussianData gd_PotentialHealthTotalDown;
    public GaussianData gd_DistanceToTowerOwnUp;
    public GaussianData gd_DistanceToTowerOwnDown;
       
    //Constructor (not necessary, but helpful)
    public TroopGaussianData(GaussianData gd_PotentialDPSTotalUp,GaussianData gd_PotentialDPSTotalDown,GaussianData gd_PotentialHealthTotalUp,GaussianData gd_PotentialHealthTotalDown,GaussianData gd_DistanceToTowerOwnUp,GaussianData gd_DistanceToTowerOwnDown) {
        this.gd_PotentialDPSTotalUp = gd_PotentialDPSTotalUp;
        this.gd_PotentialDPSTotalDown = gd_PotentialDPSTotalDown;
        this.gd_PotentialHealthTotalUp = gd_PotentialHealthTotalUp;
        this.gd_PotentialHealthTotalDown = gd_PotentialHealthTotalDown;
        this.gd_DistanceToTowerOwnUp = gd_DistanceToTowerOwnUp;
        this.gd_DistanceToTowerOwnDown = gd_DistanceToTowerOwnDown;
    }
};

public class AI_GNB : MonoBehaviour,IAI
{
    [SerializeField] private DeployCardSystem _deployCardSystem;
    [SerializeField] private CardSystem _cardSystem;
    [SerializeField] private Observer _observer;

    SituationData[] _dataSet;

    public bool isPlayer1 = true;
    public string id{get;set;}
    public float timePerThinking = 0.5f;

    public Transform towerP1Pos;
    public Transform towerP2Pos;

    private bool _stopThinking = false;

    Vector3 _spawnPoint;

    //Variable of the situation data to think of
    private SituationData _situation;

    //All the data to apply the GNB
    private Dictionary<string,TroopGaussianData> troopGaussianData = new Dictionary<string,TroopGaussianData>();

    //Spawnpoint to deply troop
    Vector3 _deployPoint = new Vector3(0.0f,0.0f,0.0f);

    // Start is called before the first frame update
    void Start()
    {
        var hash = new Hash128();
        hash.Append(Random.Range(0, 100));
        id = hash.ToString();

        BuildDataSet();// First gets the data set from a file
        CalculateBasicGaussianData();// Second gets all the data needed for the method
        StartCoroutine(CalculateAction());// Start the coroutine to think and decide the action
    }

    private void CalculateBasicGaussianData(){
        CalculateMean();
        CalculateStandardDeviation();
    }

    private void CalculateMean(){
        //Total of each troop to make the mean
        Dictionary<string,int> totalEachTroop = new Dictionary<string,int>();

        //Mean
        foreach(SituationData data in _dataSet){
            if(!troopGaussianData.ContainsKey(data.troop+":"+data.zone)){
                totalEachTroop.Add(data.troop+":"+data.zone, 1);
                GaussianData gd_PotentialDPSTotalUp = new GaussianData(data.potentialDPSTotalUp, 0.0, 0.0);
                GaussianData gd_PotentialDPSTotalDown = new GaussianData(data.potentialDPSTotalDown, 0.0, 0.0);
                GaussianData gd_PotentialHealthTotalUp = new GaussianData(data.potentialHealthTotalUp, 0.0, 0.0);
                GaussianData gd_PotentialHealthTotalDown = new GaussianData(data.potentialHealthTotalDown, 0.0, 0.0);
                GaussianData gd_DistanceToTowerOwnUp = new GaussianData(data.DistanceToTowerOwnUp, 0.0, 0.0);
                GaussianData gd_DistanceToTowerOwnDown = new GaussianData(data.DistanceToTowerOwnDown, 0.0, 0.0);
                troopGaussianData.Add(data.troop+":"+data.zone, new TroopGaussianData(gd_PotentialDPSTotalUp, gd_PotentialDPSTotalDown, gd_PotentialHealthTotalUp, gd_PotentialHealthTotalDown, gd_DistanceToTowerOwnUp, gd_DistanceToTowerOwnDown));
            }else{
                totalEachTroop[data.troop+":"+data.zone] += 1;
                TroopGaussianData temporalGD = troopGaussianData[data.troop+":"+data.zone];
                temporalGD.gd_PotentialDPSTotalUp.mean += data.potentialDPSTotalUp;
                temporalGD.gd_PotentialDPSTotalDown.mean += data.potentialDPSTotalDown;
                temporalGD.gd_PotentialHealthTotalUp.mean += data.potentialHealthTotalUp;
                temporalGD.gd_PotentialHealthTotalDown.mean += data.potentialHealthTotalDown;
                temporalGD.gd_DistanceToTowerOwnUp.mean += data.DistanceToTowerOwnUp;
                temporalGD.gd_DistanceToTowerOwnDown.mean += data.DistanceToTowerOwnDown;
                troopGaussianData[data.troop+":"+data.zone] = temporalGD;
            }
        }

        List<string> keyList = new List<string>(troopGaussianData.Keys);
        foreach(string key in keyList){
            TroopGaussianData temporalGD = troopGaussianData[key];
            temporalGD.gd_PotentialDPSTotalUp.mean /= totalEachTroop[key];
            temporalGD.gd_PotentialDPSTotalDown.mean /= totalEachTroop[key];
            temporalGD.gd_PotentialHealthTotalUp.mean /= totalEachTroop[key];
            temporalGD.gd_PotentialHealthTotalDown.mean /= totalEachTroop[key];
            temporalGD.gd_DistanceToTowerOwnUp.mean /= totalEachTroop[key];
            temporalGD.gd_DistanceToTowerOwnDown.mean /= totalEachTroop[key];
            troopGaussianData[key] = temporalGD;
        }
    }

    private void CalculateStandardDeviation(){
        Dictionary<string,int> totalEachTroop = new Dictionary<string,int>();
        foreach(SituationData data in _dataSet){
            if(!totalEachTroop.ContainsKey(data.troop+":"+data.zone)){
                totalEachTroop.Add(data.troop+":"+data.zone, 1);
            }else{
                totalEachTroop[data.troop+":"+data.zone] += 1;
            }
            TroopGaussianData temporalGD = troopGaussianData[data.troop+":"+data.zone];
            temporalGD.gd_PotentialDPSTotalUp.standarDeviation += Math.Pow(data.potentialDPSTotalUp - temporalGD.gd_PotentialDPSTotalUp.mean, 2);
            temporalGD.gd_PotentialDPSTotalDown.standarDeviation += Math.Pow(data.potentialDPSTotalDown - temporalGD.gd_PotentialDPSTotalDown.mean, 2);
            temporalGD.gd_PotentialHealthTotalUp.standarDeviation += Math.Pow(data.potentialHealthTotalUp - temporalGD.gd_PotentialHealthTotalUp.mean, 2);
            temporalGD.gd_PotentialHealthTotalDown.standarDeviation += Math.Pow(data.potentialHealthTotalDown - temporalGD.gd_PotentialHealthTotalDown.mean, 2);
            temporalGD.gd_DistanceToTowerOwnUp.standarDeviation += Math.Pow(data.DistanceToTowerOwnUp - temporalGD.gd_DistanceToTowerOwnUp.mean, 2);
            temporalGD.gd_DistanceToTowerOwnDown.standarDeviation += Math.Pow(data.DistanceToTowerOwnDown - temporalGD.gd_DistanceToTowerOwnDown.mean, 2);
            troopGaussianData[data.troop+":"+data.zone]=temporalGD;
        }

        List<string> keyList = new List<string>(troopGaussianData.Keys);
        foreach(string key in keyList){
            TroopGaussianData temporalGD = troopGaussianData[key];
            temporalGD.gd_PotentialDPSTotalUp.standarDeviation = Math.Sqrt(temporalGD.gd_PotentialDPSTotalUp.standarDeviation / totalEachTroop[key])+1;
            temporalGD.gd_PotentialDPSTotalDown.standarDeviation = Math.Sqrt(temporalGD.gd_PotentialDPSTotalDown.standarDeviation / totalEachTroop[key])+1;
            temporalGD.gd_PotentialHealthTotalUp.standarDeviation = Math.Sqrt(temporalGD.gd_PotentialHealthTotalUp.standarDeviation / totalEachTroop[key])+1;
            temporalGD.gd_PotentialHealthTotalDown.standarDeviation = Math.Sqrt(temporalGD.gd_PotentialHealthTotalDown.standarDeviation / totalEachTroop[key])+1;
            temporalGD.gd_DistanceToTowerOwnUp.standarDeviation = Math.Sqrt(temporalGD.gd_DistanceToTowerOwnUp.standarDeviation / totalEachTroop[key])+1;
            temporalGD.gd_DistanceToTowerOwnDown.standarDeviation = Math.Sqrt(temporalGD.gd_DistanceToTowerOwnDown.standarDeviation / totalEachTroop[key])+1;
            troopGaussianData[key] = temporalGD;
        }
    }
    
    private void CalculateLikelihood(){
        List<string> keyList = new List<string>(troopGaussianData.Keys);
        foreach(string key in keyList){
            TroopGaussianData temporalGD = troopGaussianData[key];
            temporalGD.gd_PotentialDPSTotalUp.likelihood = Likelihood( temporalGD.gd_PotentialDPSTotalUp.mean, temporalGD.gd_PotentialDPSTotalUp.standarDeviation, _situation.potentialDPSTotalUp);
            temporalGD.gd_PotentialDPSTotalDown.likelihood = Likelihood( temporalGD.gd_PotentialDPSTotalDown.mean, temporalGD.gd_PotentialDPSTotalDown.standarDeviation, _situation.potentialDPSTotalDown);
            temporalGD.gd_PotentialHealthTotalUp.likelihood = Likelihood( temporalGD.gd_PotentialHealthTotalUp.mean, temporalGD.gd_PotentialHealthTotalUp.standarDeviation, _situation.potentialHealthTotalUp);
            temporalGD.gd_PotentialHealthTotalDown.likelihood = Likelihood( temporalGD.gd_PotentialHealthTotalDown.mean, temporalGD.gd_PotentialHealthTotalDown.standarDeviation, _situation.potentialHealthTotalDown);
            temporalGD.gd_DistanceToTowerOwnUp.likelihood = Likelihood( temporalGD.gd_DistanceToTowerOwnUp.mean, temporalGD.gd_DistanceToTowerOwnUp.standarDeviation, _situation.DistanceToTowerOwnUp);
            temporalGD.gd_DistanceToTowerOwnDown.likelihood = Likelihood( temporalGD.gd_DistanceToTowerOwnDown.mean, temporalGD.gd_DistanceToTowerOwnDown.standarDeviation, _situation.DistanceToTowerOwnDown);
            troopGaussianData[key] = temporalGD;
        }
    }

    private double Likelihood(double m, double s, double x){
        return (1/(s*Math.Sqrt(2*Mathf.PI)))*Math.Log(Math.Abs(((Math.Pow((x-m),2))/(2*Math.Pow(s,2)))));
    }
    
    public int think(Observer observer, float budget){
        //Updates the current situation to calculate which will be based on
        UpdateSituation();

        //Calculate the likelihood of the actual situation
        CalculateLikelihood();

        //Calculate prior probability
        //p(group) = nºTotalGruop/TotalDataSet
        Dictionary<string,double> priorProbability = new Dictionary<string,double>();
        
        foreach(SituationData data in _dataSet){
            if(!priorProbability.ContainsKey(data.troop+":"+data.zone)){
                priorProbability.Add(data.troop+":"+data.zone, 1.0);
            }else{
                priorProbability[data.troop+":"+data.zone] += 1.0;
            }
        }
        List<string> keyList = new List<string>(priorProbability.Keys);
        foreach(string key in keyList){
            priorProbability[key] /= _dataSet.Length;
        }

        // Calculate the final probability based on the likelihood of all the variables
        //ln(p(group)) + ln(Likelihood(dps = x | group)) + ln(Likelihood(vida = x | group))......
        Dictionary<string,double> totalProbbability = new Dictionary<string,double>();
        foreach(string key in priorProbability.Keys){
            totalProbbability.Add(key, Math.Log(priorProbability[key]) + Math.Log(troopGaussianData[key].gd_PotentialDPSTotalUp.likelihood) + Math.Log(troopGaussianData[key].gd_PotentialDPSTotalDown.likelihood) + Math.Log(troopGaussianData[key].gd_PotentialHealthTotalUp.likelihood) + Math.Log(troopGaussianData[key].gd_PotentialHealthTotalDown.likelihood) + Math.Log(troopGaussianData[key].gd_DistanceToTowerOwnUp.likelihood) + Math.Log(troopGaussianData[key].gd_DistanceToTowerOwnDown.likelihood));
        }

        //Gets the values and sort it from ist highest
        string action="";
        string actionEqual ="";
        string actionAlternative = "";
        double previousActionValue = -9999999999999999999.0;
        foreach(string key in totalProbbability.Keys){
           actionAlternative = key;
           if(previousActionValue < totalProbbability[key]){
                action = key;
                previousActionValue = totalProbbability[key];
           }if(previousActionValue == totalProbbability[key]){
                actionEqual = key;
           }
        }

        //Gets the card index of the hand based on the name, and the position to spawn it
        int actionIndex = Random.Range(0, 4);
        string[] actions;
        //Adds a random factor for a non previsible behaviour
        if(actionIndex == 0){
            actions =  actionAlternative.Split(char.Parse(":"));
            actions[0] = _cardSystem.hand[Random.Range(0,_cardSystem.hand.Length-1)].name;
        }else if(actionIndex == 1){
            actions =  actionEqual.Split(char.Parse(":"));
        }else{
            actions =  action.Split(char.Parse(":"));
        }
        Debug.Log(actions[0]);
        int index= 0;
        bool found = false;
        if(actions.Length > 0){
            foreach(Card card in _cardSystem.hand){
                if(card.name == actions[0]){
                        found = true;
                        _spawnPoint = GetSpawnPoint(actions[1]);
                        break;
                }
                index++;
            }
        }

        //Registry the final action in the data collector and returns it
        if(found == true){
            //FindObjectsOfType<DataGameCollector>()[0].RegisterNewEntryData(id,"GNB",actions[0]+"_"+actions[1]);
            return index;
        }
        return -1;
    }

    Vector3 GetSpawnPoint(string zone){
        if(isPlayer1 == true){
            if(zone.Equals("up")){
                return new Vector3(Random.Range(-6.5f,-4.5f),Random.Range(2.6f,3.7f), 0.0f);
            }else if(zone.Equals("down")){
                return new Vector3(Random.Range(-6.5f,-4.5f),Random.Range(-3.7f,-2.6f), 0.0f);
            }else{
                return new Vector3(Random.Range(-4.9f,-3.6f),Random.Range(-0.5f,0.5f), 0.0f);
            }
        }else{
            if(zone.Equals("up")){
                return new Vector3(Random.Range(6.5f,4.5f),Random.Range(2.6f,3.7f), 0.0f);
            }else if(zone.Equals("down")){
                return new Vector3(Random.Range(6.5f,4.5f),Random.Range(-3.7f,-2.6f), 0.0f);
            }else{
                return new Vector3(Random.Range(4.9f,3.6f),Random.Range(-0.5f,0.5f), 0.0f);
            }
        }
    }

    private List<Card> GetHand(){
        if(isPlayer1 == true){
            return _observer.AvailableTroops(0);
        }else{
            return _observer.AvailableTroops(1);
        }
    }

    private IEnumerator CalculateAction(){
        while(!_stopThinking){
            
            //Thinks a action and spawn a card
            int action = think(_observer,0.0f);
            if(action != -1){
                _cardSystem.TryToPlayCardAI(action, _spawnPoint, isPlayer1);
            }
            
            //yield on a new YieldInstruction that waits for x
            yield return new WaitForSeconds(timePerThinking);
        }
    }

    void UpdateSituation(){
        _situation = new SituationData("","",1.0,1.0,1.0,1.0,13.0,13.0);
        int totalTroops = 0;

        Transform towerPos;
        if(isPlayer1 ==  true){
            towerPos = towerP1Pos;
        }else{
            towerPos = towerP2Pos;
        }

        foreach(GameObject enemy in GetEnemiesUp()){
            NPC npc = enemy.GetComponent(typeof(NPC)) as NPC;
            //Checks just in case the enemy is still alive
            if(npc.atributes.health > 0){
                _situation.potentialDPSTotalUp += (npc.atributes.attacksPerSecond * npc.atributes.damage);
                _situation.potentialHealthTotalUp += npc.atributes.health;
                _situation.DistanceToTowerOwnUp += Mathf.Abs(Vector3.Distance(towerPos.position, enemy.transform.position));
                totalTroops++;  
            }      
        }
        _situation.DistanceToTowerOwnUp /= totalTroops;

        totalTroops = 0;
        foreach(GameObject enemy in GetEnemiesDown()){
            NPC npc = enemy.GetComponent(typeof(NPC)) as NPC;
            //Checks just in case the enemy is still alive
            if(npc.atributes.health > 0){
                _situation.potentialDPSTotalDown += (npc.atributes.attacksPerSecond * npc.atributes.damage);
                _situation.potentialHealthTotalDown += npc.atributes.health;
                _situation.DistanceToTowerOwnDown += Mathf.Abs(Vector3.Distance(towerPos.position, enemy.transform.position));   
                totalTroops++;     
            }   
        }
        _situation.DistanceToTowerOwnDown /= totalTroops;
    }

    List<GameObject> GetEnemiesUp(){
        List<GameObject> listEnemies = new List<GameObject>();
        GameObject[] enemies = null;

        //Gets all the enemies
        if(isPlayer1 == true){
            _observer.TroopsInField(1);
            enemies = _observer.playersTroops[1];
        }else{
            _observer.TroopsInField(0);
            enemies = _observer.playersTroops[0];
        }

        foreach(GameObject enemy in enemies){
            if(enemy.transform.position.y >= 0){
                listEnemies.Add(enemy);
            }
        }

        return listEnemies;
    }

    List<GameObject> GetEnemiesDown(){
        List<GameObject> listEnemies = new List<GameObject>();
        GameObject[] enemies = null;

        //Gets all the enemies
        if(isPlayer1 == true){
            _observer.TroopsInField(1);
            enemies = _observer.playersTroops[1];
        }else{
            _observer.TroopsInField(0);
            enemies = _observer.playersTroops[0];
        }

        foreach(GameObject enemy in enemies){
            if(enemy.transform.position.y < 0){
                listEnemies.Add(enemy);
            }
        }

        return listEnemies;
    }


    void BuildDataSet()
    {
        StreamReader sr = new StreamReader(Path.Combine(Application.streamingAssetsPath, "DataSetGNB.csv"));
        bool eof = false;
        List<SituationData> situationData = new List<SituationData>();
        while (!eof)
        {
            string data_string = sr.ReadLine();
            if (data_string == null)
            {
                eof = true;
                break;
            }
            string[] data =  data_string.Split(char.Parse(";"));
            situationData.Add(new SituationData(data[0], data[1], Convert.ToDouble(data[2]), Convert.ToDouble(data[3]), Convert.ToDouble(data[4]), Convert.ToDouble(data[5]), Convert.ToDouble(data[6]), Convert.ToDouble(data[7])));
        }
        sr.Close();
        _dataSet = situationData.ToArray();
    }
}