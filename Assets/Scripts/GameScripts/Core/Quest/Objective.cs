using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class Objective
    {
        public ObjectiveType OType;
        public ScriptableObject TargerDetail;
        public int RequireAmount = 1;
        public int CurrentAmount = 0;
        public bool isComplete;


        public Objective(Objective objective)
        {
            OType = objective.OType;
            TargerDetail = objective.TargerDetail;
            CurrentAmount = objective.CurrentAmount;
            RequireAmount = objective.RequireAmount;
            isComplete = objective.isComplete;
        }

    }
    public enum ObjectiveType
    {
        GAIN,
        HUNT,
        TALK
    }
}
