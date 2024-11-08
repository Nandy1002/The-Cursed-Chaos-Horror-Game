using System;

namespace VladislavTsurikov.SceneDataSystem.Scripts.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    abstract class AllowInstanceAttribute : Attribute
    {
        abstract public bool Allow(SceneDataManager sceneDataManager);
    }
}