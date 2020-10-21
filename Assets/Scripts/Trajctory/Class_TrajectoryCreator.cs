using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace Trajctory
{
    #region 公共声明

    #region 委托

    #endregion 委托

    #region 枚举

    /// <summary>
    /// 弹道移动类型
    /// </summary>
    public enum EnumTrajectoryMoveType
    {
        /// <summary>
        /// 每时间间隔速度确定
        /// </summary>
        Velocity,

        /// <summary>
        /// 到达终点总时间确定
        /// </summary>
        SpentTime,
    }

    #endregion 枚举

    #region 定义

    #endregion 定义

    #endregion 公共声明

    #region 弹道创建附件

    /// <summary>
    /// 弹道创建附件
    /// </summary>
    public class Class_TrajectoryCreator : MonoBehaviour
    {
        #region 内部声明

        #region 常量

#if UNITY_EDITOR
        private const float Const_MaxShowDebugTime = 10f;
#endif

        #endregion 常量

        #region 枚举

        #endregion 枚举

        #region 定义

        #endregion 定义

        #region 委托

        #endregion 委托

#endregion 内部声明

        #region 属性字段

        #region 静态属性

        #endregion 静态属性

        #region 属性

        /// <summary>
        /// 移动器列表
        /// </summary>
        public List<Class_TrajectoryMover> ListMover { get; } = new List<Class_TrajectoryMover>();

        #endregion 属性

        #region 字段

        #region 公共字段

#if UNITY_EDITOR

        [Header("调试设置"), Tooltip("用于显示投射物在某时刻所在位置及自转朝向")]
        public float DebugProjectileTime = 0;

#endif
        /// <summary>
        /// 移动类型
        /// </summary>
        [Header("移动类型配置"), Tooltip("移动类型。Constant Velocity代表移动的速度固定；Constant Time代表移动的总耗时固定")]
        public EnumTrajectoryMoveType MoveType = EnumTrajectoryMoveType.Velocity;

        /// <summary>
        /// 速度
        /// </summary>
        [Header("轨迹配置"), Tooltip("移动速度曲线或时间消耗曲线。如果Move Type配置为Constant Velocity，则曲线X轴代表时间点，Y轴代表此时间点投射物的速度；如果配置为Constant Time，那么曲线代表时间点，Y轴代表投射物移动距离的比例，当y大于等于1时即为命中目标。")]
        public AnimationCurve VelocityOrTimeSpend = Const_Trajectory.DefaultVelocity;

        /// <summary>
        /// 半径曲线
        /// </summary>
        [Tooltip("投射物偏离发射点到目标点的直线垂直距离，若投射物所在的极坐标平面垂直于此直线，且直线经过坐标原点，那么配置数据X轴代表时间，Y轴代表投射物在坐标平面上的点到极坐标平面原点的半径r")]
        public AnimationCurve Radius = Const_Trajectory.DefaultRadius;

        /// <summary>
        /// 轨迹旋转曲线
        /// </summary>
        [Tooltip("投射物偏离发射点到目标点的直线垂线的旋转角度，若投射物所在的极坐标平面垂直于此直线，且直线经过坐标原点，那么配置数据X轴代表时间，Y轴代表投射物在坐标平面上的点的旋转就角度γ。极坐标0度永远平行于世界坐标的XZ平面。")]
        public AnimationCurve TrajectoryRotation = Const_Trajectory.DefaultTrajectoryRotation;

        /// <summary>
        /// 投射物自转曲线
        /// </summary>
        [Tooltip("投射物自身的旋转角度，投射物的朝向永远面向目标点，旋转0度角永远平行于世界坐标的XZ平面。")]
        public AnimationCurve ProjectileRotation = Const_Trajectory.DefaultProjectileRotation;

        /// <summary>
        /// 锁定投射物朝向到目标
        /// </summary>
        [Tooltip("True投射物始终面向目标，False，投射物朝向始终平行发射点到目标单位所在位置的直线上")]
        public bool AlwaysFaceTarget = false;

        /// <summary>
        /// 投射物最大存在时间
        /// </summary>
        [Tooltip("投射物最大寿命")]
        public float LifeTime = 30;

        /// <summary>
        /// 失控定时
        /// </summary>
        [Tooltip("超过时间后，投射物仅受速度或耗时影响，直线飞向目标对象，负数代表永远不失控。恒定耗时模式下无效")]
        public float TimeOfLostControl = -1;

        /// <summary>
        /// 投射物
        /// </summary>
        [Tooltip("投射物Prefab")]
        public GameObject Projectile;

        /// <summary>
        /// 发射特效
        /// </summary>
        [Tooltip("发射特效Prefab")]
        public GameObject LaunchEffect;

        /// <summary>
        /// 轰击特效
        /// </summary>
        [Tooltip("轰击特效Prefab")]
        public GameObject ImpactEffect;

        /// <summary>
        /// 目标对象
        /// </summary>
        [Tooltip("目标对象")]
        public GameObject TargetObject;

        #endregion 公共字段

        #region 私有字段

        #endregion 私有字段

        #endregion 字段

        #region 事件

        #endregion 事件

        #endregion 属性字段

        #region 构造函数

        #endregion 构造函数

        #region 方法

        #region 通用方法

        /// <summary>
        /// 移动器销毁
        /// </summary>
        /// <param name="mover">移动器</param>
        public void OnEventDesotroy(Class_TrajectoryMover mover)
        {
            if (ListMover.Contains(mover))
            {
                ListMover.Remove(mover);
            }
        }

        /// <summary>
        /// 发射
        /// </summary>
        /// <param name="target">目标</param>
        public void Launch(GameObject target)
        {
            if (TargetObject != target) TargetObject = target;
            Quaternion direction = Quaternion.LookRotation(TargetObject.transform.position - transform.position);
            GameObject projectile = Instantiate(Projectile, transform.position, direction);
            Class_TrajectoryMover mover = projectile.AddComponent<Class_TrajectoryMover>();
            if (LaunchEffect != null)
            {
                GameObject lanuch = Instantiate(LaunchEffect, transform.position, direction);
            }
            ListMover.Add(mover);
            mover.CopyFromSimulator(this);
            mover.EventOnDestroy += OnEventDesotroy;
        }

        #endregion 通用方法

        #region 重写方法

#if UNITY_EDITOR

        /// <summary>
        /// 选择渲染
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            float time = 0f;
            Vector3 targetPos = TargetObject == null ? transform.forward.normalized * 10 + transform.position : TargetObject.transform.position;
            Vector3 newOriginalPos = transform.position;
            Vector3 newProjectilePos;
            Quaternion projectileRotation;
            Const_Trajectory.Move_UpdateProjectilePosAndRotation(TrajectoryRotation, Radius, ProjectileRotation, time, newOriginalPos, out newProjectilePos, out projectileRotation, newOriginalPos, targetPos, AlwaysFaceTarget);
            Vector3 preOriginalPos = newOriginalPos;
            Vector3 preProjectilePos = newProjectilePos;
            bool hit = false;
            while (!hit)
            {
                float debugInterval = time - DebugProjectileTime;
                if (debugInterval <= 0 && Mathf.Abs(time - DebugProjectileTime) < Time.fixedDeltaTime)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(newProjectilePos, 0.5f);
                    Vector3 polarAxis = -Vector3.Cross((targetPos - transform.position), Vector3.up).normalized;
                    Vector3 showDirect = (projectileRotation * polarAxis).normalized * 1;
                    Gizmos.DrawLine(newProjectilePos, newProjectilePos + showDirect);
                }

                hit = Const_Trajectory.Move(MoveType, VelocityOrTimeSpend, TrajectoryRotation, Radius, ProjectileRotation, time, ref newOriginalPos, ref newProjectilePos, out projectileRotation, transform.position, targetPos, AlwaysFaceTarget, TimeOfLostControl);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(preOriginalPos,newOriginalPos);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(preProjectilePos, newProjectilePos);

                time += Time.fixedDeltaTime;
                preOriginalPos = newOriginalPos;
                preProjectilePos = newProjectilePos;
                if (time > LifeTime) break;
            }
        }

#endif

        /// <summary>
        /// 固定更新
        /// </summary>
        private void Update()
        {
            if (TargetObject == null) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Launch(TargetObject);
            }
        }

        #endregion 重写方法

        #region 事件方法

        #endregion 事件方法 

        #endregion 方法
    }

    #endregion 弹道创建附件
}
