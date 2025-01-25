using UnityEngine;

public class MenuRunner : MonoBehaviour
{
    public void Play()
    {
        AppManager.Instance.Play();
    }

    public void Credits()
    {
        AppManager.Instance.Credits();
    }

    public void Quit()
    {
        AppManager.Instance.Quit();
    }
}
