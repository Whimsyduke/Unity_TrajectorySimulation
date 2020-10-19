using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        /// 每时间间隔速度恒定
        /// </summary>
        ConstantVelocity,

        /// <summary>
        /// 到达终点总时间恒定
        /// </summary>
        ConstantTime,
    }

    #endregion 枚举

    #region 定义

    #endregion 定义

    #endregion 公共声明

    #region 弹道模拟附件

    /// <summary>
    /// 弹道模拟附件
    /// </summary>
    public class Class_TrajectorySimulator : MonoBehaviour
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
        [Header("轨迹配置"), Tooltip("移动速度曲线或时间消耗曲线。如果Move Type配置为Constant Velocity，则曲线X轴代表时间点，Y轴代表此时间点投射物的速度；如果配置为Constant Time，那么曲线对X轴的投影")]
        public AnimationCurve VelocityOrTimeSpend;

        /// <summary>
        /// X轴高度曲线
        /// </summary>
        public AnimationCurve HeightX;

        /// <summary>
        /// Y轴高度曲线
        /// </summary>
        public AnimationCurve HeightY;

        /// <summary>
        /// 轨迹旋转曲线
        /// </summary>
        public AnimationCurve TrackRotation;

        /// <summary>
        /// 投射物自转曲线曲线
        /// </summary>
        public AnimationCurve ProjectileRotation;

        /// <summary>
        /// 投射物
        /// </summary>
        public GameObject Projectile;

        /// <summary>
        /// 发射特效
        /// </summary>
        public GameObject LaunchEffect;

        /// <summary>
        /// 轰击特效
        /// </summary>
        public GameObject ImpaceEffect;

        #endregion 字段

        #region 事件

        #endregion 事件

        #endregion 属性字段

        #region 构造函数

        #endregion 构造函数

        #region 方法

        #region 通用方法

        

        #endregion 通用方法

        #region 重写方法

        /// <summary>
        /// 启动
        /// </summary>
        private void Start()
        {
            
        }

        #endregion 重写方法

        #region 事件方法

        #endregion 事件方法 

        #endregion 方法
    }

    #endregion 弹道模拟附件
}
