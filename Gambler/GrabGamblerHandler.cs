using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#if DEBUG
namespace MoreSparks.Gambler;

internal class GrabGamblerHandler : MonoBehaviour
{
    public static GrabGamblerHandler instance;

    private const int SUCCESS_AMOUNT = 500;
    private const int FAIL_AMOUNT = 650;

    private int counterSuccess;

    public int CounterSuccess
    {
        get { return counterSuccess; }
        set { counterSuccess = value; }
    }

    private int counterFail;

    public int CounterFail
    {
        get { return counterFail; }
        set { counterFail = value; }
    }

    private void Awake()
    {
        instance = this;
    }

    public int GetAmount()
    {
        int winings = counterSuccess * SUCCESS_AMOUNT;
        int losses = counterFail * FAIL_AMOUNT;

        int amount = winings + -losses;
        MainMoreSparks.Logger.LogWarning("Have Earned: " + amount);
        return amount;
    }
}

#endif