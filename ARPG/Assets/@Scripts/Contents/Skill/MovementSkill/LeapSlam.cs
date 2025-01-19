using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapSlam : MovementSkill
{
    public Vector3 startPoint;  // ���� ����
    public Vector3 endPoint;    // ���� ����
    public float arcHeight = 2.0f;  // ������ �ְ� ����
    public float durationDefault = 0.8f;  // �̵� �ð�
    public float duration = 0.8f;  // �̵� �ð�

    private float timeElapsed = 0f;  // ��� �ð�

    bool IsMoving = false;

    public override void DoSkill(Vector2 target)
    {
        if (IsMoving) return;

        base.DoSkill(target);


        if (Owner.CreatureState != Define.ECreatureState.Skill)
            return;

        startPoint = gameObject.transform.position;
        endPoint = target;
        IsMoving = true;

        if (Owner._animation != null)
        {
            Owner._animation.Jump();
        }


        duration = durationDefault / AttackSpeedMultiplier;
    }

    protected override void OnAttackEvent()
    {
        Vector3 _skillDir = (Target - Owner.transform.position).normalized;
        int _angleRange = 180;
        float radius = Util.GetEffectRadius(SkillData.EffectSize);
        List<Creature> targets = Managers.Object.FindConeRangeTargets(Owner, _skillDir, radius, _angleRange);

        foreach (var target in targets)
        {
            if (target.IsValid())
            {
                target.OnDamaged(Owner, this);
            }
        }



        if (Owner.Anim == null)
        {
            OnAttackEndEvent();
        }
    }

    protected override void OnAttackEndEvent()
    {
        Owner.OnAnimAttackEnded -= OnAttackEndEvent;

        if (Owner.CreatureState == Define.ECreatureState.Skill)
        {
            Owner.CreatureState = Define.ECreatureState.Move;
        }
    }

    void Update()
    {
        if (IsMoving)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;

            // t ���� 0���� 1 ���̸� �����ϵ��� Ŭ����
            t = Mathf.Clamp01(t);

            // ������ ��� ���
            Vector3 currentPos = Parabola(startPoint, endPoint, arcHeight, t);
            transform.position = currentPos;

            // �̵� �Ϸ� �� ��� �ð� �ʱ�ȭ (�ɼ�)
            if (t >= 1f)
            {
                timeElapsed = 0.0f; // �Ǵ� �̵� �Ϸ� ó��
                MoveEnd();
            }
        }
    }

    public override bool CanSkill()
    {
        return !IsMoving;
    }

    void MoveEnd()
    {
        Vector3Int cellPos = Managers.Map.World2Cell(endPoint);
        GetComponent<Creature>().SetCellPos(cellPos, true);
        IsMoving = false;
        Owner.CreatureState = Define.ECreatureState.Idle;

    }

    Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        // �߰� ���� ���
        Vector3 mid = Vector3.Lerp(start, end, t);

        // t�� ���� ������ ���� ���
        float y = height * (1 - 4 * (t - 0.5f) * (t - 0.5f));

        // y �� ����
        return new Vector3(mid.x, y + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
}
