using UnityEngine;
using static HelpPhrasesModule;

/// <summary>
/// �����, ���������� �� �������������� � ��������: "��������"
/// </summary>
public class Shotgun : MonoBehaviour, IInteractable
{
    private string helpPhrase = actionToPhrase[Action.PickUp];

    /// <summary>
    /// �����, �������������� �������������� � ��������
    /// </summary>
    public void Interact()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// �����, ��������������� ��������� ��� �������
    /// </summary>
    /// <returns>��������� ��� �������</returns>

    public string GetHelpPhrase()
    {
        return helpPhrase;
    }
}