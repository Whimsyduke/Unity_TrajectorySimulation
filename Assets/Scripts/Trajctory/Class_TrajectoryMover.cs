using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Trajctory
{
    #region 弹道信息

    /// <summary>
    /// 弹道信息
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

        #endregion 委托

        #endregion 内部声明

        #region 属性字段

        #region 静态属性

        #endregion 静态属性

        #region 属性

        #endregion 属性

        #region 字段

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
        public AnimationCurve TrackRotation;

        /// <summary>
        /// 投射物自转曲线
        /// </summary>
        [Tooltip("投射物自身的旋转角度，投射物的朝向永远面向目标点，旋转0度角永远平行于世界坐标的XZ平面。")]
        public AnimationCurve ProjectileRotation;

        /// <summary>
        /// 轰击特效
        /// </summary>
        [Tooltip("轰击特效Prefab")]
        public GameObject ImpaceEffect;

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

        /// <summary>
        /// 存在时间
        /// </summary>
        private float mCreateTime = 0;

        /// <summary>
        /// 在发射点和目标点直线投影位置
        /// </summary>
        private Vector3 mOriginalPos;

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
        public event Action<Class_TrajectoryMover, Vector3 , Vector3, Vector3, Vector3> EventOnMove;

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
                        VelocityOrTimeSpend = Class_TrajectoryCreator.DefaultVelocity;
                        break;
                    case EnumTrajectoryMoveType.SpentTime:
                        VelocityOrTimeSpend = Class_TrajectoryCreator.DefaultTimeSpand;
                        break;
                    default:
                        throw new Exception();
                }
            }
            else
            {
                VelocityOrTimeSpend = simulator.VelocityOrTimeSpend;
            }
            Radius = simulator.Radius != null ? simulator.Radius : Class_TrajectoryCreator.DefaultHeight;
            TrackRotation = simulator.TrackRotation != null ? simulator.TrackRotation : Class_TrajectoryCreator.DefaultTrackRotation;
            ProjectileRotation = simulator.ProjectileRotation != null ? simulator.ProjectileRotation : Class_TrajectoryCreator.DefaultProjectileRotation;
            ImpaceEffect = simulator.ImpaceEffect;
            TargetObject = simulator.TargetObject;
            LaunchPos = simulator.transform.position;
            mOriginalPos = LaunchPos;
            mCreateTime = Time.fixedTime;
            EventOnDestroy += simulator.OnMoverDesotroy;
        }

        #region 移动

        /// <summary>
        /// 速度确定移动
        /// </summary>
        /// <param name="time">移动时间</param>
        /// <param name="originalPos">投影点坐标</param>
        /// <param name="targetPos">目标点坐标</param>
        /// <returns>命中</returns>
        protected bool Move_Velocity(float time, ref Vector3 originalPos, Vector3 targetPos)
        {
            float speedVal = VelocityOrTimeSpend.Evaluate(time);
            Vector3 velocity = (targetPos - LaunchPos).normalized * speedVal;
            if ((targetPos - originalPos).sqrMagnitude <= velocity.sqrMagnitude)
            {
                return true;
            }
            else
            {
                originalPos = originalPos + velocity;
                return false;
            }
        }

        /// <summary>
        /// 耗时确定移动
        /// </summary>
        /// <param name="time">移动时间</param>
        /// <param name="originalPos">投影点坐标</param>
        /// <param name="targetPos">目标点坐标</param>
        /// <returns>命中</returns>
        protected bool Move_SpentTime(float time, ref Vector3 originalPos, Vector3 targetPos)
        {
            float posVal = VelocityOrTimeSpend.Evaluate(time);
            if (posVal > 1)
            {
                return true;
            }
            else
            {
                originalPos = Vector3.Lerp(LaunchPos, targetPos, posVal);
                return false;
            }
        }

        /// <summary>
        /// 刷新投射物位置
        /// </summary>
        /// <param name="time">移动时间</param>
        /// <param name="projectilePos">投射物坐标</param>
        /// <param name="targetPos">目标点坐标</param>
        protected void Move_UpdateProjectile(float time, ref Vector3 projectilePos, Vector3 targetPos)
        {
            Vector3 polarAxis = -Vector3.Cross((targetPos - LaunchPos), Vector3.up).normalized;
            float angle = TrackRotation.Evaluate(time);
            float radius = Radius.Evaluate(time);
            Quaternion rotation = Quaternion.Euler((float)Math.Cos(angle), (float)Math.Sin(angle), 0);
            Vector3 direction = (rotation * polarAxis).normalized;
            projectilePos += direction * radius;
        }

        /// <summary>
        /// 投射物移动
        /// </summary>
        /// <param name="time">移动时间</param>
        /// <param name="originalPos">投影点坐标</param>
        /// <param name="projectilePos">投射物坐标</param>
        /// <param name="targetPos">目标点坐标</param>
        protected void Move(float time, ref Vector3 originalPos, ref Vector3 projectilePos, Vector3 targetPos)
        {
            Vector3 preOrigin = originalPos;
            Vector3 prePos = projectilePos;
            bool hit;
            switch (MoveType)
            {
                case EnumTrajectoryMoveType.Velocity:
                    hit = Move_Velocity(time, ref originalPos, targetPos);
                    break;
                case EnumTrajectoryMoveType.SpentTime:
                    hit = Move_SpentTime(time, ref originalPos, targetPos);
                    break;
                default:
                    throw new Exception();
            }
            if (hit)
            {
                EventOnHit?.Invoke(this);
                return;
            }
            Move_UpdateProjectile(time, ref projectilePos, targetPos);
            Vector3 newOrigin = originalPos;
            Vector3 newPos = projectilePos;
            EventOnMove?.Invoke(this, preOrigin, prePos, newOrigin, newPos);
        }

        /// <summary>
        /// 移动
        /// </summary>
        protected void Move()
        {
            Vector3 pos = transform.position;
            Move(Time.fixedTime - mCreateTime, ref mOriginalPos, ref pos, TargetObject.transform.position);
            transform.position = pos;
        }

        #endregion 移动

        #endregion 通用方法

        #region 重写方法

        /// <summary>
        /// 移动器销毁
        /// </summary>
        private void OnDestroy()
        {
            EventOnDestroy?.Invoke(this);
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

    #endregion 弹道信息
}
