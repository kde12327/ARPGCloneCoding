using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapSlam : MovementSkill
{
    public Vector3 startPoint;  // 시작 지점
    public Vector3 endPoint;    // 도착 지점
    public float arcHeight = 2.0f;  // 포물선 최고 높이
    public float durationDefault = 0.8f;  // 이동 시간
    public float duration = 0.8f;  // 이동 시간

    private float timeElapsed = 0f;  // 경과 시간

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

            // t 값이 0에서 1 사이를 유지하도록 클램프
            t = Mathf.Clamp01(t);

            // 포물선 경로 계산
            Vector3 currentPos = Parabola(startPoint, endPoint, arcHeight, t);
            transform.position = currentPos;

            // 이동 완료 시 경과 시간 초기화 (옵션)
            if (t >= 1f)
            {
                timeElapsed = 0.0f; // 또는 이동 완료 처리
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
        // 중간 지점 계산
        Vector3 mid = Vector3.Lerp(start, end, t);

        // t에 따른 포물선 높이 계산
        float y = height * (1 - 4 * (t - 0.5f) * (t - 0.5f));

        // y 값 적용
        return new Vector3(mid.x, y + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
}
