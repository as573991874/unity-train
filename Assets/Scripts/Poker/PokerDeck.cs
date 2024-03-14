using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PokerDeck {
    // 牌组
    private PokerCard[] cards;

    // 初始化牌组
    public void Init() {
        this.cards = new PokerCard[52];
        for (uint i = 0; i < cards.Length; i++) {
            this.cards[i] = new PokerCard(i + 1);
        }
    }

    // 洗牌
    public void Shuffle() {
        System.Random rd = new System.Random();
        for (int i = cards.Length - 1; i > 0; i--) {
            int j = rd.Next(i + 1);
            PokerCard temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    // 从头获取指定数量的牌组
    public List<PokerCard> GetCards(int cardCount) {
        List<PokerCard> cards = new List<PokerCard>();
        for (int i = 0; i < cardCount; i++) {
            cards.Add(this.cards[i]);
        }
        return cards;
    }

    // 输出
    public void PrintCards() {
        StringBuilder sb = new StringBuilder();
        foreach (PokerCard card in cards) {
            sb.Append(card.ToString()).Append(" ");
        }
        Debug.Log(sb.ToString());
    }
}
