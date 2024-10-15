using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomGridLayoutGroup : GridLayoutGroup
{
    public override void SetLayoutHorizontal()
    {
        base.SetLayoutHorizontal();
        SetCells();
    }

    public override void SetLayoutVertical()
    {
        base.SetLayoutVertical();
        SetCells();
    }

    private void SetCells()
    {
        int rowCount = Mathf.CeilToInt((float)rectChildren.Count / constraintCount); // �� �� �� ���
        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform item = rectChildren[i];
            int row = i / constraintCount; // ���� ��
            int col = i % constraintCount; // ���� ��

            // ¦�� ���� ���ʿ��� ������, Ȧ�� ���� �����ʿ��� �������� ��ġ
            if (row % 2 == 0)
            {
                // ¦�� ��: �⺻ ��ġ
                SetChildAlongAxis(item, 0, col * (cellSize.x + spacing.x));
            }
            else
            {
                // Ȧ�� ��: �����ʿ��� �������� ��ġ
                int reverseCol = constraintCount - 1 - col;
                SetChildAlongAxis(item, 0, reverseCol * (cellSize.x + spacing.x));
            }

            // ���� ��ġ�� �״�� ����
            SetChildAlongAxis(item, 1, row * (cellSize.y + spacing.y));
        }
    }
}