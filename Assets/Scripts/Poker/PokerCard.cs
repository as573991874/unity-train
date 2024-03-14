using System;
using System.Text;
using UnityEngine;

// 花色
public enum PokerSuit {
    Spades = 0, // 黑桃
    Clubs = 1, // 梅花
    Diamonds = 2, // 方片
    Hearts = 3 // 红心
}

// 数字
public enum PokerRank {
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

public class PokerCard {
    private uint id;
    public PokerRank rank { private set; get; }
    public PokerSuit suit { private set; get; }
    private string desc;

    public PokerCard(uint id) {
        if (id == 0 || id > 52) {
            Debug.LogError("PokerCard id error");
        }
        this.id = id;
        this.rank = (PokerRank)(id % 13 == 0 ? 13 : id % 13);
        this.suit = (PokerSuit)((id - 1) / 13);

        StringBuilder sb = new StringBuilder();
        switch (this.suit) {
            case PokerSuit.Spades:
                sb.Append("♠");
                break;
            case PokerSuit.Clubs:
                sb.Append("♣");
                break;
            case PokerSuit.Diamonds:
                sb.Append("♦");
                break;
            case PokerSuit.Hearts:
                sb.Append("♥");
                break;
            default:
                break;
        }

        switch (this.rank) {
            case PokerRank.Ace:
                sb.Append("A");
                break;
            case PokerRank.Two:
                sb.Append("2");
                break;
            case PokerRank.Three:
                sb.Append("3");
                break;
            case PokerRank.Four:
                sb.Append("4");
                break;
            case PokerRank.Five:
                sb.Append("5");
                break;
            case PokerRank.Six:
                sb.Append("6");
                break;
            case PokerRank.Seven:
                sb.Append("7");
                break;
            case PokerRank.Eight:
                sb.Append("8");
                break;
            case PokerRank.Nine:
                sb.Append("9");
                break;
            case PokerRank.Ten:
                sb.Append("10");
                break;
            case PokerRank.Jack:
                sb.Append("J");
                break;
            case PokerRank.Queen:
                sb.Append("Q");
                break;
            case PokerRank.King:
                sb.Append("K");
                break;
            default:
                break;
        }
        this.desc = sb.ToString();
    }

    public static int GetRankScore(PokerRank rank) {
        if (rank == PokerRank.Ace) {
            return 14;
        } else {
            return (int)rank;
        }
    }

    public override string ToString() {
        return this.desc;
    }
}
