using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrizGames.Vour
{
    /// <summary>
    /// EmptyLocation
    /// </summary>
    public class EmptyLocation : LocationBase
    {
        public override void UpdateLocation() => IsReady = true;
    }
}