using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    private List<Worm> _worms = new List<Worm>();
    private Color _color;

    public void Init(List<Worm> worms, Color color)
    {
        _worms = worms;
        _color = color;

        foreach (var worm in _worms)
        {
            worm.Init(color);
        }
    }

    public void StartTurn()
    {
        _worms[0].EnableInput();
    }
}
