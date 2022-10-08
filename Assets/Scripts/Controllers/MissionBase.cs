using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public enum MissionType
{
    SingleRun, TotalMeters, CoinSingleRun
}
//Classe abstrata de Missões
public abstract class MissionBase : MonoBehaviour
{
    public int max;
    public int progress;
    public int reward;
    public PlayerRun player;
    public int currentProgress;
    public MissionType missionType;

    public abstract void Create();
    public abstract string GetMissionDescription();
    public abstract void RunStart();
    public abstract void Update();
    public bool GetMissionComplete()
    {
        if ((progress + currentProgress) >= max)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
//Missões simples de corrida
public class SingleRun : MissionBase
{
    public override void Create()
    {
        missionType = MissionType.SingleRun;
        int[] maxValues = { 1000, 2000, 4000, 6000 };
        int randomMaxValue = Random.Range(0, maxValues.Length);
        int[] rewards = { 100, 200, 400, 600 };
        reward = rewards[randomMaxValue];
        max = maxValues[randomMaxValue];
        progress = 0;
    }

    public override string GetMissionDescription()
    {
        return "Run" + max + "M in one race";
    }

    public override void RunStart()
    {
        if (GameController._gameController.is_continue == true)
        {
            progress += currentProgress;
            player = FindObjectOfType<PlayerRun>();
        }
        else
        {
            progress = 0;
            player = FindObjectOfType<PlayerRun>();
        }
    }

    public override void Update()
    {
        if (player == null)
        {
            return;
        }
        currentProgress = (int)player.score;
    }
}
//Missões Cumulativas de corrida
public class TotalMeters : MissionBase
{
    public override void Create()
    {
        missionType = MissionType.TotalMeters;
        int[] maxValues = { 10000, 20000, 40000, 60000 };
        int randomMaxValue = Random.Range(0, maxValues.Length);
        int[] rewards = { 1000, 2000, 4000, 6000 };
        reward = rewards[randomMaxValue];
        max = maxValues[randomMaxValue];
        progress = 0;
    }

    public override string GetMissionDescription()
    {
        return "Accumulate " + max + "M running";
    }

    public override void RunStart()
    {
        progress += currentProgress;
        player = FindObjectOfType<PlayerRun>();
    }

    public override void Update()
    {
        if (player == null)
        {
            return;
        }
        currentProgress = (int)player.score;
    }
}
//Missões Simples de Coleta
public class CoinSingleRun : MissionBase
{
    public override void Create()
    {
        missionType = MissionType.CoinSingleRun;
        int[] maxValues = { 350, 850, 1050, 1400 };
        int randomMaxValue = Random.Range(0, maxValues.Length);
        int[] rewards = { 250, 500, 750, 950 };
        reward = rewards[randomMaxValue];
        max = maxValues[randomMaxValue];
        progress = 0;
    }

    public override string GetMissionDescription()
    {
        return "Collect " + max + " coins in a race";
    }

    public override void RunStart()
    {
        if (GameController._gameController.is_continue == true)
        {
            progress += currentProgress;
            player = FindObjectOfType<PlayerRun>();
        }
        else
        {
            progress = 0;
            player = FindObjectOfType<PlayerRun>();
        }
    }

    public override void Update()
    {
        if (player == null)
        {
            return;
        }
        currentProgress = (int)player.coin;
    }
}