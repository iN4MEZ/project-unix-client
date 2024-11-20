using NMX.GameCore.Network.Client;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace NMX
{
    public interface IResult
    {
        bool NextPacket([MaybeNullWhen(false)] out NetPacket packet);


    }
}
