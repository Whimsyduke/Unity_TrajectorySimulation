using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Trajctory
{
    #region 弹道移动附件

    /// <summary>
    /// 弹道移动附件
    /// </summary>
    public class Class_TrajectoryMover : MonoBehaviour
    {
        #region 内部声明

        #region 常量

        #endregion 常量

        #region 枚举

        #endregion 枚举

        #region 定义

        #endregion 定义

        #region 委托

        /// <summary>
        /// 移动事件委托
        /// </summary>
        /// <param name="mover">移动器</param>
        /// <param name="preOriginalPos">前一投影位置</param>
        /// <param name="prePrejectilePos">前一投射物位置</param>
        /// <param name="newOriginalPos">新投影位置</param>
        /// <param name="newPrejectilePos">新投射物位置</param>
        /// <param name="prejectileRotation">投射物自旋</param>
        public delegate void FuncMove(Class_TrajectoryMover mover, Vector3 preOriginalPos, Vector3 prePrejectilePos, Vector3 newOriginalPos, Vector3 newPrejectilePos, Quaternion prejectileRotation);

        #endregion 委托

        #endregion 内部声明

        #region 属性字段

        #region 静态属性

        #endregion 静态属性

        #region 属性

        #endregion 属性

        #region 字段

        #region 公共字段

        /// <summary>
        /// 移动类型
        /// </summary>
        [Header("移动类型配置"), Tooltip("移动类型。Constant Velocity代表移动的速度固定；Constant Time代表移动的总耗时固定")]
        public EnumTrajectoryMoveType MoveType;

        /// <summary>
        /// 速度
        /// </summary>
        [Header("轨迹配置"), Tooltip("移动速度曲线或时间消耗曲线。如果Move Type配置为Constant Velocity，则曲线X轴代表时间点，Y轴代表此时间点投射物的速度；如果配置为Constant Time，那么曲线代表时间点，Y轴代表投射物移动距离的比例，当y大于等于1时即为命中目标。")]
        public AnimationCurve VelocityOrTimeSpend;

        /// <summary>
        /// 半径曲线
        /// </summary>
        [Tooltip("投射物偏离发射点到目标点的直线垂直距离，若投射物所在的极坐标平面垂直于此直线，且直线经过坐标原点，那么配置数据X轴代表时间，Y轴代表投射物在坐标平面上的点到极坐标平面原点的半径r")]
        public AnimationCurve Radius;

        /// <summary>
        /// 轨迹旋转曲线
        /// </summary>
        [Tooltip("投射物偏离发射点到目标点的直线垂线的旋转角度，若投射物所在的极坐标平面垂直于此直线，且直线经过坐标原点，那么配置数据X轴代表时间，Y轴代表投射物在坐标平面上的点的旋转就角度γ。极坐标0度永远平行于世界坐标的XZ平面。")]
        public AnimationCurve TrajectoryRotation;

        /// <summary>
        /// 投射物自转曲线
        /// </summary>
        [Tooltip("投射物自身的旋转角度，投射物的朝向永远面向目标点，旋转0度角永远平行于世界坐标的XZ平面。")]
        public AnimationCurve ProjectileRotation;

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
        [Tooltip("超过时间后，投射物仅受速度或耗时影响，直线飞向目标对象，负数代表永远不失控。。恒定耗时模式下无效")]
        public float TimeOfLostControl = -1;

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

        /// <summary>
        /// 发射位置
        /// </summary>
        [Tooltip("发射位置")]
        public Vector3 LaunchPos;

        #endregion 公共字段

        #region 私有字段

        /// <summary>
        /// 存在时间
        /// </summary>
        private float mCreateTime = 0;

        /// <summary>
        /// 在发射点和目标点直线投影位置
        /// </summary>
        private Vector3 mOriginalPos;

        #endregion 私有字段

        #endregion 字段

        #region 事件

        /// <summary>
        /// 移动器销毁事件
        /// </summary>
        public event Action<Class_TrajectoryMover> EventOnDestroy;

        /// <summary>
        /// 命中事件
        /// </summary>
        public event Action<Class_TrajectoryMover> EventOnHit;

        /// <summary>
        /// 移动事件事件，参数分别为移动器，原投影位置，原投射物位置，新投影位置，新投射物位置
        /// </summary>
        public event FuncMove EventOnMove;

        /// <summary>
        /// 超过存在时间事件
        /// </summary>
        public event Action<Class_TrajectoryMover> EventOnOutOfLifeTime;

        #endregion 事件

        #endregion 属性字段

        #region 构造函数

        #endregion 构造函数

        #region 方法

        #region 通用方法

        /// <summary>
        /// 从创建器复制数据
        /// </summary>
        /// <param name="simulator">创建器</param>
        public void CopyFromSimulator(Class_TrajectoryCreator simulator)
        {
            MoveType = simulator.MoveType;
            if (simulator.VelocityOrTimeSpend == null)
            {
                switch (MoveType)
                {
                    case EnumTrajectoryMoveType.Velocity:
                        VelocityOrTimeSpend = Const_Trajectory.DefaultVelocity;
                        break;
                    case EnumTrajectoryMoveType.SpentTime:
                        VelocityOrTimeSpend = Const_Trajectory.DefaultTimeSpand;
                        break;
                    default:
                        throw new Exception();
                }
            }
            else
            {
                VelocityOrTimeSpend = simulator.VelocityOrTimeSpend;
            }
            Radius = simulator.Radius != null ? simulator.Radius : Const_Trajectory.DefaultRadius;
            TrajectoryRotation = simulator.TrajectoryRotation != null ? simulator.TrajectoryRotation : Const_Trajectory.DefaultTrajectoryRotation;
            ProjectileRotation = simulator.ProjectileRotation != null ? simulator.ProjectileRotation : Const_Trajectory.DefaultProjectileRotation;
            ImpactEffect = simulator.ImpactEffect;
            TargetObject = simulator.TargetObject;
            AlwaysFaceTarget = simulator.AlwaysFaceTarget;
            LifeTime = simulator.LifeTime;
            TimeOfLostControl = simulator.TimeOfLostControl;
            LaunchPos = simulator.transform.position;
            mOriginalPos = LaunchPos;
            mCreateTime = Time.fixedTime;
        }

        /// <summary>
        /// 移动
        /// </summary>
        protected void Move()
        {
            float time = Time.fixedTime - mCreateTime;
            if (time > LifeTime)
            {
                if (EventOnOutOfLifeTime != null) EventOnOutOfLifeTime.Invoke(this);
                Destroy(gameObject);
                return;
            }
            Vector3 originalPos = mOriginalPos;
            Vector3 projectilePos = transform.position;
            Vector3 targetPos = TargetObject.transform.position;
            Quaternion projectileRotation;
            bool hit = Const_Trajectory.Move(MoveType, VelocityOrTimeSpend, TrajectoryRotation, Radius, ProjectileRotation, time, ref originalPos, ref projectilePos, out projectileRotation, LaunchPos, targetPos, AlwaysFaceTarget, TimeOfLostControl);
            if (EventOnMove != null) EventOnMove.Invoke(this, mOriginalPos, transform.position, originalPos, projectilePos, projectileRotation);
            mOriginalPos = originalPos;
            transform.position = projectilePos;
            float angle;
            Vector3 direction;
            projectileRotation.ToAngleAxis(out angle, out direction);
            transform.rotation = Quaternion.FromToRotation(Vector3.forward, targetPos - LaunchPos) * Quaternion.Euler(0, 0, angle);
            Debug.Log(transform.rotation.eulerAngles);
            if (hit)
            {
                if (TargetObject != null)
                {
                    Instantiate(ImpactEffect, TargetObject.transform.position, transform.rotation);
                }
                if (EventOnHit != null) EventOnHit.Invoke(this);
                DestroyImmediate(gameObject);
            }
        }

        #endregion 通用方法

        #region 重写方法

        /// <summary>
        /// 移动器销毁
        /// </summary>
        private void OnDestroy()
        {
            if (EventOnDestroy != null) EventOnDestroy.Invoke(this);
        }

        /// <summary>
        /// 移动器更新
        /// </summary>
        private void FixedUpdate()
        {
            if (TargetObject == null)
            {
                Destroy(gameObject);
                return;
            }
            Move();
        }

        #endregion 重写方法

        #region 事件方法

        #endregion 事件方法 

        #endregion 方法
    }

    #endregion 弹道移动附件
}
