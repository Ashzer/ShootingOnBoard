﻿using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public interface ISelector
    {
        void Check(Ray ray , string selectableTag);
        Transform GetSelection();
    }
}