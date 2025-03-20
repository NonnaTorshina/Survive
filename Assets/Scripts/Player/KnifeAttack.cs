using System.Collections;
using UnityEngine;

/// <summary>
/// �����, ���������� �� ����� �������� �������
/// </summary>
public class KnifeAttack : MonoBehaviour
{
    /// <summary>
    /// ����������, ���������� �� ������� ������ �����
    /// </summary>
    private bool isAttacking = false;

    /// <summary>
    /// ������, ���������� ��������
    /// </summary>
    public GameObject attackingRange;

    /// <summary>
    /// ��������, ���������� �� �������� �����
    /// </summary>
    public Animator attackingAnimator;

    /// <summary>
    /// �����, �������� ��������� �������
    /// </summary>
    private void ChangeVisibility() => attackingRange.SetActive(isAttacking);

    /// <summary>
    /// �����, ������������ ������ ������� ����
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            ChangeVisibility();
        }

        if (isAttacking && attackingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            isAttacking = false;
            ChangeVisibility();
        }
    }
}