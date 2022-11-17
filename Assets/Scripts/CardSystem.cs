using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    Vector3 OFFSET = new Vector3(20, 10, 0);

    [SerializeField] Card[] initialDeck = new Card[8];
    [SerializeField] Transform deckTransform;

    [SerializeField] Transform cardHeightToBePlayed;

    public Card[] hand = new Card[4];
    public Queue<Card> deck = new Queue<Card>(4);

    public Transform[] cardPositions = new Transform[4];

    [SerializeField] DeployCardSystem deployCardSystem;

    private void Start()
    {
        Card[] randomDeck = initialDeck.OrderBy(x => Random.Range(0, initialDeck.Length - 1)).ToArray();

        for (int i = 0, j = 0; i < randomDeck.Length; i++, j++)
        {
            hand[j] = randomDeck[i];
            hand[j].handIndex = j;
            deck.Enqueue(randomDeck[++i]);
        }

        for (int i = 0; i < hand.Length; i++)
        {
            hand[i].transform.position = cardPositions[i].position;
        }
    }

    public void TryToPlayCard(int playedCardIndex)
    {
        Card playedCard = hand[playedCardIndex];

        if ((playedCard.canBePlayed || playedCard.rightClick) && Camera.main.ScreenToWorldPoint(Input.mousePosition).y > cardHeightToBePlayed.position.y &&
            deployCardSystem.CanDeploy(playedCard))
        {
            PlayCard(playedCard);
            deployCardSystem.DeployCard(playedCard);
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
}