using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenuWindow : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _animator.SetTrigger("Show");
    }

    public void Hide() 
    { 
        _animator.SetTrigger("Hide");
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
