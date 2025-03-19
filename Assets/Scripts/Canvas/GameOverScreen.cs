using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    /// <summary>
    /// �����, ��������� ����� "Game Over"
    /// </summary>
    public void Setup()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// �����, ���������� ���� ������, ��� ������� �� ������ "Restart"
    /// </summary>
    public void RestartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
