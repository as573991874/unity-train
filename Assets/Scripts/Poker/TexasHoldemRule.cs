

using System.Collections.Generic;
using System.Linq;
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
    public List<PokerCard> cards { private set; get; }

    public HandRankResult(List<PokerCard> cards) {

        this.cards = cards;
        if (cards.Count < 5) {
            this.rank = HandRank.None;
            this.score = 0;
        } else {
            this.Calculate();
        }
    }

    private void Calculate() {
        var rankCounts = cards.GroupBy(card => card.rank).ToDictionary(g => g.Key, g => g.Count());
        var isFlush = cards.Select(card => card.suit).Distinct().Count() == 1;
        var sortedRanks = cards.Select(card => card.rank).OrderBy(rank => PokerCard.GetRankScore(rank)).ToArray();
        var isStraight = Enumerable.Range(0, 4).All(i => sortedRanks[i + 1] - sortedRanks[i] == 1);

        if (isStraight && isFlush) {
            if (sortedRanks[4] == PokerRank.Ace && sortedRanks[0] == PokerRank.Ten) {
                this.rank = HandRank.RoyalFlush;
                this.score = 9000000;
                return;
            } else {
                this.rank = HandRank.StraightFlush;
                this.score = 8000000 + PokerCard.GetRankScore(sortedRanks[4]);
                return;
            }
        }

        if (rankCounts.ContainsValue(4)) {
            this.rank = HandRank.FourOfKind;
            this.score = 7000000 + PokerCard.GetRankScore(rankCounts.First(kv => kv.Value == 4).Key) * 20 + PokerCard.GetRankScore(rankCounts.First(kv => kv.Value == 1).Key);
            return;
        }

        if (rankCounts.ContainsValue(3) && rankCounts.ContainsValue(2)) {
            this.rank = HandRank.FullHouse;
            this.score = 6000000 + PokerCard.GetRankScore(rankCounts.First(kv => kv.Value == 3).Key) * 20 + PokerCard.GetRankScore(rankCounts.First(kv => kv.Value == 2).Key);
            return;
        }

        if (isFlush) {
            this.rank = HandRank.Flush;
            this.score = 5000000 + PokerCard.GetRankScore(sortedRanks[4]);
            return;
        }

        if (isStraight) {
            this.rank = HandRank.Straight;
            this.score = 4000000 + PokerCard.GetRankScore(sortedRanks[4]);
            return;
        }

        if (rankCounts.ContainsValue(3)) {
            this.rank = HandRank.ThreeOfKind;
            this.score = 3000000 + PokerCard.GetRankScore(rankCounts.First(kv => kv.Value == 3).Key);
            return;
        }

        if (rankCounts.Count(kv => kv.Value == 2) == 2) {
            this.rank = HandRank.TwoPair;
            var pairs = rankCounts.Where(kv => kv.Value == 2).OrderByDescending(kv => PokerCard.GetRankScore(kv.Key)).Select(kv => PokerCard.GetRankScore(kv.Key)).ToList();
            this.score = 2000000 + pairs[0] * 20 + pairs[1];
            return;
        }

        if (rankCounts.ContainsValue(2)) {
            this.rank = HandRank.OnePair;
            this.score = 1000000 + PokerCard.GetRankScore(rankCounts.First(kv => kv.Value == 2).Key);
            return;
        }

        this.rank = HandRank.HighCard;
        this.score = PokerCard.GetRankScore(sortedRanks[4]) * 20000 + PokerCard.GetRankScore(sortedRanks[3]) * 2000 + PokerCard.GetRankScore(sortedRanks[2]) * 200 + PokerCard.GetRankScore(sortedRanks[1]) * 20 + PokerCard.GetRankScore(sortedRanks[0]);
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < cards.Count; i++) {
            sb.Append(cards[i].ToString()).Append(" ");
        }

        switch (rank) {
            case HandRank.HighCard:
                sb.Append("HighCard");
                break;
            case HandRank.OnePair:
                sb.Append("OnePair");
                break;
            case HandRank.TwoPair:
                sb.Append("TwoPair");
                break;
            case HandRank.ThreeOfKind:
                sb.Append("ThreeOfKind");
                break;
            case HandRank.Straight:
                sb.Append("Straight");
                break;
            case HandRank.Flush:
                sb.Append("Flush");
                break;
            case HandRank.FullHouse:
                sb.Append("FullHouse");
                break;
            case HandRank.FourOfKind:
                sb.Append("FourOfKind");
                break;
            case HandRank.StraightFlush:
                sb.Append("StraightFlush");
                break;
            case HandRank.RoyalFlush:
                sb.Append("RoyalFlush");
                break;
            default:
                break;
        }

        sb.Append(" ").Append(score);

        return sb.ToString();
    }
}

public class TexasHoldemRule {

    // 计算牌型结果
    public HandRankResult Calculate(List<PokerCard> cards) {
        int cardCount = 5;
        if (cards.Count < cardCount) {
            return new HandRankResult(cards);
        }

        var combinations = GetCombinations(cards, cardCount);
        Debug.Log(string.Format("GetCombinations: {1}, From {0}", cards.Count, combinations.Count));
        foreach (List<PokerCard> combination in combinations) {
            var result = new HandRankResult(combination);
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
