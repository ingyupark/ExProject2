using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Click_Up,
        Drag,
        BinginDrag,
        EndDrag,
        Drop
    }

    public enum UIChatEvent
    {
        Return,
    }
}
