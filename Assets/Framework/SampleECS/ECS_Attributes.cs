﻿using System;

namespace SampleECS
{
    /// <summary>
    /// ECS Component
    /// </summary>
    public class ComponentAttribute : Attribute
    {
        public bool isStatic = false;
        public string context = "";
        public ComponentAttribute(string context, bool isStatic = false)
        {
            this.context = context;
            this.isStatic = isStatic;
        }
    }

    public enum SystemMode
    {
        /// <summary>
        /// 当实体组件触发信号时，才会调用Exsute。
        /// </summary>
        Action,
        /// <summary>
        /// 每帧调用Excute
        /// </summary>
        Loop,
    }

    /// <summary>
    /// ECS System
    /// </summary>
    public class SystemAttribute : Attribute
    {
        public SystemMode systemMode = SystemMode.Action;
        public string context;
        public SystemAttribute() { }
        public SystemAttribute(string context) { systemMode = SystemMode.Action; this.context = context; }
        public SystemAttribute(SystemMode systemMode, string context) { this.systemMode = systemMode; this.context = context; }
    }
}
