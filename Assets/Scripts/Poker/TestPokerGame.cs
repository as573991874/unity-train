using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPokerGame : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnGUI() {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Test")) {
            PokerDeck pd = new PokerDeck();
            pd.Init();
            pd.Shuffle();
            pd.PrintCards();
        }

        if (GUI.Button(new Rect(0, 110, 100, 100), "Test2")) {
            PokerDeck pd = new PokerDeck();
            pd.Init();
            pd.Shuffle();
            TexasHoldemRule thr = new TexasHoldemRule();
            thr.Calculate(pd.GetCards(5));
        }

        if (GUI.Button(new Rect(0, 220, 100, 100), "Test3")) {
            PokerDeck pd = new PokerDeck();
            pd.Init();
            pd.Shuffle();
            TexasHoldemRule thr = new TexasHoldemRule();
            thr.Calculate(pd.GetCards(8));
        }
    }
}
