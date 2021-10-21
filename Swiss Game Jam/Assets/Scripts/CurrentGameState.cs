using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurrentGameState
{
    public static GameState state = GameState.STARTDIALOGUE;
    public static Sword[] swords = new Sword[3];
    public static int currentSwordIndex = 0;

    public static int score = 0;

    public static Sword CurrentSword()
    {
        return swords[currentSwordIndex];
    }

    public static void ResetSwordUsages()
    {
        foreach (Sword sword in swords)
        {
            if (sword != null)
                sword.usagesLeft = sword.baseUsage + sword.addedUsage;
        }
    }
}
