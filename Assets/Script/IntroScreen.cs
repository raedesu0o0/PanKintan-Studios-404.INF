using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScreenScript : MonoBehaviour
{
    // This method should be connected to the "Begin Sequence" button's OnClick event
    public void OnBeginSequenceClick()
    {
        SceneManager.LoadScene("Level One");
    }
}
