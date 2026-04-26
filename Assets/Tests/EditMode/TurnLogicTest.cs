//Written by: Vincent Luong

using NUnit.Framework;
using UnityEngine;

public class TurnLogicTest
{
    [Test]
    public void PlayerTurn()
    {
        TurnLogicManager turnLogic = new TurnLogicManager(); //create a new instance of the TurnLogicManager class

        turnLogic.NextTurn(); // turn 1

        Assert.IsTrue(turnLogic.IsPlayerTurn()); //returns true because it's the player's turn
    }

    [Test]
    public void EnemyTurn()
    {
        TurnLogicManager EnemyTurnLogic = new TurnLogicManager(); //create a new instance of the TurnLogicManager class

        EnemyTurnLogic.NextTurn(); //turn 1
        EnemyTurnLogic.NextTurn(); //turn 2

        Assert.IsFalse(EnemyTurnLogic.IsPlayerTurn()); //returns false because it's the enemy's turn
    }

    [Test]
    public void TurnLogicTestCorrectly()
    {
        TurnLogicManager turnLogicTest = new TurnLogicManager(); //create a new instance of the TurnLogicManager class

        turnLogicTest.NextTurn(); //turn 1
        Assert.IsTrue(turnLogicTest.IsPlayerTurn()); //returns true because it's the player's turn
        Debug.Log("Turn 1: This is currently the player's turn!");

        turnLogicTest.NextTurn(); //turn 2
        Assert.IsFalse(turnLogicTest.IsPlayerTurn()); //returns false because it's the enemy's turn
        Debug.Log("Turn 2: It is now the enemy's turn!");

        turnLogicTest.NextTurn(); //turn 3
        Assert.IsTrue(turnLogicTest.IsPlayerTurn()); //returns true because it's the player's turn
    }
}
