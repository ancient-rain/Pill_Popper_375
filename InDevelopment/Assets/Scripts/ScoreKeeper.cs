using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour {

    public static int score { get; private set; }
    float lastTimeEnemyWasKilled;
    int currentStreakCount;
    float streakExp = 1;

    void Start()
    {
        score = 0;
        Enemy.onDeathStatic += onEnemyKilled;
        FindObjectOfType<Player>().onDeath += onPlayerDeath;
    }

    void onEnemyKilled()
    {
        if(Time.time < lastTimeEnemyWasKilled + streakExp)
        {
            currentStreakCount++;
        }
        else
        {
            currentStreakCount = 1;
        }
        lastTimeEnemyWasKilled = Time.time;
        score += 5 * currentStreakCount;
    }

    void onPlayerDeath()
    {
        Enemy.onDeathStatic -= onEnemyKilled;
    }
}
