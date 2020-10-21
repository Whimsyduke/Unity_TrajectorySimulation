using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Other
{
    #region 对象寿命

    /// <summary>
    /// 对象寿命
    /// </summary>
    public class Class_LifeTime : MonoBehaviour
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
        /// 对象寿命
        /// </summary>
        public float LifeTime = 1;

        private float mCreateTime;

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
        /// 对象初始化
        /// </summary>
        private void Start()
        {
            mCreateTime = Time.fixedTime;
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        private void FixedUpdate()
        {
            if(Time.fixedTime - mCreateTime > LifeTime)
            {
                Destroy(gameObject);
            }
        }

        #endregion 重写方法

        #region 事件方法

        #endregion 事件方法 

        #endregion 方法
    }

    #endregion 对象寿命
}
