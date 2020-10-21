using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Trajctory
{
    #region 弹道模拟通用数据

    #region 公共声明

    #region 委托

    #endregion 委托

    #region 枚举

    #endregion 枚举

    #region 定义

    #endregion 定义

    #endregion 公共声明

    #region 公共类

    /// <summary>
    /// 
    /// </summary>
    public static class Const_Trajectory
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
        public static AnimationCurve DefaultRadius { get; } = AnimationCurve.Constant(0, 10, 0);

        /// <summary>
        /// 默认轨迹旋转
        /// </summary>
        public static AnimationCurve DefaultTrajectoryRotation { get; } = AnimationCurve.Constant(0, 10, 0);

        /// <summary>
        /// 默认投射物自转
        /// </summary>
        public static AnimationCurve DefaultProjectileRotation { get; } = AnimationCurve.Constant(0, 10, 0);

        #endregion 静态属性

        #region 属性

        #endregion 属性

        #region 字段

        #endregion 字段

        #region 事件

        #endregion 事件

        #endregion 属性字段

        #region 构造函数

        #endregion 构造函数

        #region 方法

        #region 通用方法

        #region 移动

        /// <summary>
        /// 速度确定移动
        /// </summary>
        /// <param name="velocityCurve">速度曲线</param>
        /// <param name="time">移动时间</param>
        /// <param name="originalPos">投影点坐标</param>
        /// <param name="launchPos">发射点坐标</param>
        /// <param name="targetPos">目标点坐标</param>
        /// <returns>命中</returns>
        private static bool Move_Velocity(AnimationCurve velocityCurve, float time, ref Vector3 originalPos, Vector3 launchPos, Vector3 targetPos)
        {
            float speedVal = velocityCurve.Evaluate(time) * Time.fixedDeltaTime;
            Vector3 velocity = (targetPos - launchPos).normalized * speedVal;
            originalPos += velocity;
            if ((targetPos - launchPos).sqrMagnitude <= (originalPos - launchPos).sqrMagnitude)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 耗时确定移动
        /// </summary>
        /// <param name="positionCurve">位置曲线</param>
        /// <param name="time">移动时间</param>
        /// <param name="originalPos">投影点坐标</param>
        /// <param name="launchPos">发射点坐标</param>
        /// <param name="targetPos">目标点坐标</param>
        /// <returns>命中</returns>
        private static bool Move_SpentTime(AnimationCurve positionCurve, float time, ref Vector3 originalPos, Vector3 launchPos, Vector3 targetPos)
        {
            float posVal = positionCurve.Evaluate(time);
            if (posVal >= 1)
            {
                return true;
            }
            else
            {
                originalPos = Vector3.Lerp(launchPos, targetPos, posVal);
                return false;
            }
        }

        /// <summary>
        /// 刷新投射物位置及旋转
        /// </summary>
        /// <param name="trajectoryRotationCurve">轨迹旋转曲线</param>
        /// <param name="radiusCurve">半径曲线</param>
        /// <param name="projectilerRotationCurve">投射物旋转曲线</param>
        /// <param name="time">移动时间</param>
        /// <param name="originalPos">投影坐标</param>
        /// <param name="launchPos">发射点坐标</param>
        /// <param name="targetPos">目标点坐标</param>
        /// <param name="alwaysFaceTarget">锁定投射物朝向到目标</param>
        public static void Move_UpdateProjectilePosAndRotation(AnimationCurve trajectoryRotationCurve, AnimationCurve radiusCurve, AnimationCurve projectilerRotationCurve, float time, Vector3 originalPos, out Vector3 projectilePos, out Quaternion projectileRotation, Vector3 launchPos, Vector3 targetPos, bool alwaysFaceTarget)
        {
            Vector3 polarAxis = -Vector3.Cross((targetPos - launchPos), Vector3.up).normalized;
            float radius = radiusCurve.Evaluate(time);
            float angle = trajectoryRotationCurve.Evaluate(time);
            Quaternion rotation = Quaternion.AngleAxis(angle, targetPos - launchPos);
            Vector3 direction = (rotation * polarAxis).normalized;
            projectilePos = originalPos + direction * radius;
            projectileRotation = Move_UpdateProjectileRotation(projectilerRotationCurve, time, projectilePos, launchPos,targetPos, alwaysFaceTarget);
        }

        /// <summary>
        /// 刷新投射物旋转
        /// </summary>
        /// <param name="projectilerRotationCurve">投射物旋转曲线</param>
        /// <param name="time">移动时间</param>
        /// <param name="projectilePos">投射物坐标</param>
        /// <param name="launchPos">发射点坐标</param>
        /// <param name="targetPos">目标点坐标</param>
        /// <param name="alwaysFaceTarget">锁定投射物朝向到目标</param>
        /// <returns>投射物旋转角度</returns>
        private static Quaternion Move_UpdateProjectileRotation(AnimationCurve projectilerRotationCurve, float time, Vector3 projectilePos, Vector3 launchPos, Vector3 targetPos, bool alwaysFaceTarget)
        {
            float angle = projectilerRotationCurve.Evaluate(time);
            if (alwaysFaceTarget)
            {
                return Quaternion.AngleAxis(angle, targetPos - projectilePos);
            }
            else
            {
                return Quaternion.AngleAxis(angle, targetPos - launchPos);
            }
        }

        /// <summary>
        /// 投射物移动
        /// </summary>
        /// <param name="moveType">移动类型</param>
        /// <param name="velocityOrPosCurve">速度或位置曲线</param>
        /// <param name="trajectoryRotationCurve">轨迹旋转曲线</param>
        /// <param name="radiusCurve">半径曲线</param>
        /// <param name="projectilerRotationCurve">投射物旋转曲线</param>
        /// <param name="time">移动时间</param>
        /// <param name="originalPos">投影点坐标</param>
        /// <param name="projectilePos">投射物坐标</param>
        /// <param name="launchPos">发射点坐标</param>
        /// <param name="targetPos">目标点坐标</param>
        /// <param name="alwaysFaceTarget">锁定投射物朝向到目标</param>
        /// <param name="lostControlTime">失控定时</param>
        public static bool Move(EnumTrajectoryMoveType moveType, AnimationCurve velocityOrPosCurve, AnimationCurve trajectoryRotationCurve, AnimationCurve radiusCurve, AnimationCurve projectilerRotationCurve, float time, ref Vector3 originalPos, ref Vector3 projectilePos, out Quaternion projectileRotation, Vector3 launchPos, Vector3 targetPos, bool alwaysFaceTarget, float lostControlTime)
        {
            bool hit;
            if (moveType == EnumTrajectoryMoveType.SpentTime || lostControlTime < 0 || time <= lostControlTime)
            {
                switch (moveType)
                {
                    case EnumTrajectoryMoveType.Velocity:
                        hit = Move_Velocity(velocityOrPosCurve, time, ref originalPos, launchPos, targetPos);
                        break;
                    case EnumTrajectoryMoveType.SpentTime:
                        hit = Move_SpentTime(velocityOrPosCurve, time, ref originalPos, launchPos, targetPos);
                        break;
                    default:
                        throw new Exception();
                }
                if (hit)
                {
                    projectileRotation = new Quaternion();
                    return true;
                }
                Move_UpdateProjectilePosAndRotation(trajectoryRotationCurve, radiusCurve, projectilerRotationCurve, time, originalPos, out projectilePos, out projectileRotation, launchPos, targetPos, alwaysFaceTarget);
            }
            else
            {
                Vector3 templaunchPos = projectilePos;
                hit = Move_Velocity(velocityOrPosCurve, time, ref projectilePos, templaunchPos, targetPos);
                if (hit)
                {
                    projectileRotation = new Quaternion();
                    return true;
                }
                projectileRotation = Move_UpdateProjectileRotation(projectilerRotationCurve, time, projectilePos, launchPos, targetPos, alwaysFaceTarget);
            }
            return false;
        }

        #endregion 移动

        #endregion 通用方法

        #region 重写方法

        #endregion 重写方法

        #region 事件方法

        #endregion 事件方法 

        #endregion 方法
    }

    #endregion 公共类

    #endregion 弹道模拟通用数据
}
