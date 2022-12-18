using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    Vector3 OFFSET = new Vector3(20, 10, 0);

    [SerializeField] Card[] initialDeck = new Card[6];
    [SerializeField] Transform deckTransform;

    [SerializeField] Transform minCardPosToBePlayed;
    [SerializeField] Transform maxCardPosToBePlayed;


    [HideInInspector] public Card[] hand = new Card[4];
    public Queue<Card> deck = new Queue<Card>(2);

    [HideInInspector] public Transform[] cardPositions = new Transform[4];

    [SerializeField] DeployCardSystem deployCardSystem;

    bool isAI;

    private void Start()
    {
        if (cardPositions.Length == 0)
            isAI = true;
        else
            isAI = false;


        Card[] randomDeck = initialDeck.OrderBy(x => Random.Range(0, initialDeck.Length - 1)).ToArray();

        for (int i = 0; i < hand.Length; i++)
        {
            hand[i] = randomDeck[i];
            hand[i].handIndex = i;
        }

        for (int i = hand.Length; i < randomDeck.Length; i++)
            deck.Enqueue(randomDeck[i]);

        if (!isAI)
        {
            for (int i = 0; i < hand.Length; i++)
            {
                hand[i].transform.position = cardPositions[i].position;
            }
        }
    }

    public void TryToPlayCardAI(int playedCardIndex)
    {
        Card playedCard = hand[playedCardIndex];

        Vector3 pos = RandomPositionBetween(minCardPosToBePlayed.position, maxCardPosToBePlayed.position);

        if (deployCardSystem.CanDeploy(playedCard))
        {
            PlayCardAI(playedCard);
            deployCardSystem.DeployCardAI(playedCard, pos);
        }
        else
        {
            Debug.Log("AI --> Not enough elixir");
        }
    }

    public void TryToPlayCardAI(int playedCardIndex, Vector3 pos)
    {
        Card playedCard = hand[playedCardIndex];

        if (deployCardSystem.CanDeploy(playedCard))
        {
            PlayCardAI(playedCard);
            deployCardSystem.DeployCardAI(playedCard, pos);
        }
        else
        {
            Debug.Log("AI --> Not enough elixir");
        }
    }

    public void TryToPlayCard(int playedCardIndex, Vector3 pos)
    {
        Card playedCard = hand[playedCardIndex];

        if (IsPosBetween(pos, minCardPosToBePlayed.position, maxCardPosToBePlayed.position) && deployCardSystem.CanDeploy(playedCard))
        {
            PlayCard(playedCard);
            deployCardSystem.DeployCard(playedCard, pos);
        }
        else
        {
            playedCard.transform.position = cardPositions[playedCardIndex].position;
        }
    }

    private void PlayCard(Card playedCard)
    {
        Card nextCard = deck.Dequeue();
        deck.Enqueue(playedCard);

        hand[playedCard.handIndex].transform.position = deckTransform.position + OFFSET;

        hand[playedCard.handIndex] = nextCard;
        nextCard.handIndex = playedCard.handIndex;

        hand[playedCard.handIndex].transform.position = cardPositions[playedCard.handIndex].position;
    }

    private void PlayCardAI(Card playedCard)
    {
        Card nextCard = deck.Dequeue();
        deck.Enqueue(playedCard);

        hand[playedCard.handIndex] = nextCard;
        nextCard.handIndex = playedCard.handIndex;
    }

    private bool IsPosBetween(Vector3 pos, Vector3 minPos, Vector3 maxPos)
    {
        return pos.x > minPos.x && pos.x < maxPos.x && pos.y > minPos.y && pos.y < maxPos.y;
    }

    private Vector3 RandomPositionBetween(Vector3 minPos, Vector3 maxPos)
    {
        Vector3 result = Vector3.zero;

        result.x = Random.Range(minPos.x, maxPos.x);
        result.y = Random.Range(minPos.y, maxPos.y);
        //result.z = 0;

        return result;
    }
    
    public void AlejandroTryToPlayCard(Card card, Vector3 pos, bool team)
    {
        if (deployCardSystem.CanDeploy(card))
        {
            Card nextCard = deck.Dequeue();
            deck.Enqueue(card);

            hand[card.handIndex] = nextCard;
            nextCard.handIndex = card.handIndex;
            
            deployCardSystem.DeployCardGeneral(card, pos, team);
        }
        else
        {
            Debug.Log("AI --> Not enough elixir");
        }
    }
}
