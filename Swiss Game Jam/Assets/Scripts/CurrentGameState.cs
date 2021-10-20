using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurrentGameState
{
    public static GameState state = GameState.STARTDIALOGUE;
    public static Sword[] swords = new Sword[3];
    public static int currentSwordIndex = 0;

    public static Sword CurrentSword()
    {
        return swords[currentSwordIndex];
    }
}
