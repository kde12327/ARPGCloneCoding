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
        int rowCount = Mathf.CeilToInt((float)rectChildren.Count / constraintCount); // 총 줄 수 계산
        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform item = rectChildren[i];
            int row = i / constraintCount; // 현재 줄
            int col = i % constraintCount; // 현재 열

            // 짝수 줄은 왼쪽에서 오른쪽, 홀수 줄은 오른쪽에서 왼쪽으로 배치
            if (row % 2 == 0)
            {
                // 짝수 줄: 기본 배치
                SetChildAlongAxis(item, 0, col * (cellSize.x + spacing.x));
            }
            else
            {
                // 홀수 줄: 오른쪽에서 왼쪽으로 배치
                int reverseCol = constraintCount - 1 - col;
                SetChildAlongAxis(item, 0, reverseCol * (cellSize.x + spacing.x));
            }

            // 세로 배치는 그대로 유지
            SetChildAlongAxis(item, 1, row * (cellSize.y + spacing.y));
        }
    }
}