

using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 牌型
public enum HandRank {
    None = 0, // 无牌型
    HighCard = 1, // 高牌
    OnePair = 2, // 一对
    TwoPair = 3, // 两对
    ThreeOfKind = 4, // 三条
    Straight = 5, // 顺子
    Flush = 6, // 同花
    FullHouse = 7, // 葫芦
    FourOfKind = 8, // 四条
    StraightFlush = 9, // 同花顺
    RoyalFlush = 10 // 皇家同花顺
}

// 牌型结果
public class HandRankResult {
    public HandRank rank { private set; get; }
    public int score { private set; get; }
    public PokerCard[] cards { private set; get; }

    public HandRankResult(HandRank rank, int score, PokerCard[] cards) {
        this.rank = rank;
        this.score = score;
        this.cards = cards;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < cards.Length; i++) {
            sb.Append(cards[i].ToString()).Append(" ");
        }
        return sb.ToString();
    }
}

public class TexasHoldemRule {

    // 计算牌型结果
    public HandRankResult Calculate(List<PokerCard> cards) {
        int cardCount = 5;
        if (cards.Count < cardCount) {
            return new HandRankResult(HandRank.None, 0, cards.ToArray());
        }

        var combinations = GetCombinations(cards, cardCount);
        Debug.Log(string.Format("GetCombinations: {1}, From {0}", cards.Count, combinations.Count));
        foreach (List<PokerCard> combination in combinations) {
            var result = new HandRankResult(HandRank.None, 0, combination.ToArray());
            Debug.Log(result.ToString());
        }

        return null;
    }

    // 获取所有牌组的排列组合
    private List<List<PokerCard>> GetCombinations(List<PokerCard> cards, int cardCount) {
        List<List<PokerCard>> combinations = new List<List<PokerCard>>();
        List<PokerCard> combination = new List<PokerCard>();
        GenerateCombinations(cards, cardCount, combination, 0, combinations);
        return combinations;
    }

    // 递归生成组合数组
    static void GenerateCombinations(List<PokerCard> cards, int cardCount, List<PokerCard> combination, int start, List<List<PokerCard>> combinations) {
        if (cardCount == 0) {
            combinations.Add(new List<PokerCard>(combination));
            return;
        }

        for (int i = start; i <= cards.Count - cardCount; i++) {
            combination.Add(cards[i]);
            GenerateCombinations(cards, cardCount - 1, combination, i + 1, combinations);
            combination.RemoveAt(combination.Count - 1);
        }
    }
}
