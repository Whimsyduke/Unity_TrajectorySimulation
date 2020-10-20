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

    #region 弹道模拟附件

    /// <summary>
    /// 弹道模拟附件
    /// </summary>
    public class Class_TrajectoryCreator : MonoBehaviour
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

        /// <summary>
        /// 默认速度
        /// </summary>
        public static AnimationCurve DefaultVelocity { get; } = AnimationCurve.Constant(0, 10, 10);

        /// <summary>
        /// 默认耗时
        /// </summary>
        public static AnimationCurve DefaultTimeSpand { get; } = AnimationCurve.Linear(0, 0, 10, 1);

        /// <summary>
        /// 默认高度
        /// </summary>
        public static AnimationCurve DefaultHeight { get; } = AnimationCurve.Constant(0, 10, 0);

        /// <summary>
        /// 默认轨迹旋转
        /// </summary>
        public static AnimationCurve DefaultTrackRotation { get; } = AnimationCurve.Constant(0, 10, 0);

        /// <summary>
        /// 默认投射物自转
        /// </summary>
        public static AnimationCurve DefaultProjectileRotation { get; } = AnimationCurve.Constant(0, 10, 0);

        #endregion 静态属性

        #region 属性

        /// <summary>
        /// 移动器列表
        /// </summary>
        public List<Class_TrajectoryMover> ListMover { get; } = new List<Class_TrajectoryMover>();

        #endregion 属性

        #region 字段

        /// <summary>
        /// 移动类型
        /// </summary>
        [Header("移动类型配置"), Tooltip("移动类型。Constant Velocity代表移动的速度固定；Constant Time代表移动的总耗时固定")]
        public EnumTrajectoryMoveType MoveType = EnumTrajectoryMoveType.Velocity;

        /// <summary>
        /// 速度
        /// </summary>
        [Header("轨迹配置"), Tooltip("移动速度曲线或时间消耗曲线。如果Move Type配置为Constant Velocity，则曲线X轴代表时间点，Y轴代表此时间点投射物的速度；如果配置为Constant Time，那么曲线代表时间点，Y轴代表投射物移动距离的比例，当y大于等于1时即为命中目标。")]
        public AnimationCurve VelocityOrTimeSpend = DefaultVelocity;

        /// <summary>
        /// 半径曲线
        /// </summary>
        [Tooltip("投射物偏离发射点到目标点的直线垂直距离，若投射物所在的极坐标平面垂直于此直线，且直线经过坐标原点，那么配置数据X轴代表时间，Y轴代表投射物在坐标平面上的点到极坐标平面原点的半径r")]
        public AnimationCurve Radius;

        /// <summary>
        /// 轨迹旋转曲线
        /// </summary>
        [Tooltip("投射物偏离发射点到目标点的直线垂线的旋转角度，若投射物所在的极坐标平面垂直于此直线，且直线经过坐标原点，那么配置数据X轴代表时间，Y轴代表投射物在坐标平面上的点的旋转就角度γ。极坐标0度永远平行于世界坐标的XZ平面。")]
        public AnimationCurve TrackRotation = DefaultTrackRotation;

        /// <summary>
        /// 投射物自转曲线
        /// </summary>
        [Tooltip("投射物自身的旋转角度，投射物的朝向永远面向目标点，旋转0度角永远平行于世界坐标的XZ平面。")]
        public AnimationCurve ProjectileRotation = DefaultProjectileRotation;

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
        public GameObject ImpaceEffect;

        /// <summary>
        /// 目标对象
        /// </summary>
        [Tooltip("目标对象")]
        public GameObject TargetObject;

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
        public void OnMoverDesotroy(Class_TrajectoryMover mover)
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
            GameObject projectile = Instantiate(Projectile);
            Class_TrajectoryMover mover = projectile.AddComponent<Class_TrajectoryMover>();
            ListMover.Add(mover);
            mover.CopyFromSimulator(this);
        }

        #endregion 通用方法

        #region 重写方法

        #endregion 重写方法

        #region 事件方法

        #endregion 事件方法 

        #endregion 方法
    }

    #endregion 弹道模拟附件
}
