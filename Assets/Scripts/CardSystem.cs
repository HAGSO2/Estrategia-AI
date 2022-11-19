using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    Vector3 OFFSET = new Vector3(20, 10, 0);

    [SerializeField] Card[] initialDeck = new Card[8];
    [SerializeField] Transform deckTransform;

    //[SerializeField] Transform minCardPosToBePlayed;
    //[SerializeField] Transform maxCardPosToBePlayed;

    public Card[] hand = new Card[4];
    public Queue<Card> deck = new Queue<Card>(4);

    public Transform[] cardPositions = new Transform[4];

    [SerializeField] DeployCardSystem deployCardSystem;

    bool isAI;

    private void Start()
    {
        if (cardPositions.Length == 0)
            isAI = true;
        else
            isAI = false;


        Card[] randomDeck = initialDeck.OrderBy(x => Random.Range(0, initialDeck.Length - 1)).ToArray();

        for (int i = 0, j = 0; i < randomDeck.Length; i++, j++)
        {
            hand[j] = randomDeck[i];
            hand[j].handIndex = j;
            deck.Enqueue(randomDeck[++i]);
        }

        if (!isAI)
        {
            for (int i = 0; i < hand.Length; i++)
            {
                hand[i].transform.position = cardPositions[i].position;
            }
        }
    }

    public void TryToPlayCardAI(int playedCardIndex, Vector3 pos)
    {
        Card playedCard = hand[playedCardIndex];

        PlayCardAI(playedCard);
        deployCardSystem.DeployCardAI(playedCard, pos);
    }

    public void TryToPlayCard(int playedCardIndex, Vector3 pos)
    {
        Card playedCard = hand[playedCardIndex];

        //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //if ((playedCard.canBePlayed || playedCard.rightClick) && pos.y > minCardPosToBePlayed.position.y && pos.y < maxCardPosToBePlayed.position.y &&
        //    pos.x > minCardPosToBePlayed.position.x && pos.x < maxCardPosToBePlayed.position.x && deployCardSystem.CanDeploy(playedCard))

        if ((playedCard.canBePlayed || playedCard.rightClick) && deployCardSystem.CanDeploy(playedCard))
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
}
