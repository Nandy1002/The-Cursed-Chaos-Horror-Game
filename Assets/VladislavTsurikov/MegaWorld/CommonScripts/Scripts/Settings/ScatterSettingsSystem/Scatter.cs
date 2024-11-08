using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using Component = VladislavTsurikov.ComponentStack.Scripts.Component;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem
{
    public abstract class Scatter : Component
    {
	    private const int NextFrameAfterCount = 3000;
	    private int _count;

	    public abstract void Samples(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null);
	    public abstract IEnumerator SamplesCoroutine(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null);

	    protected bool IsNeedCallNextFrame()
        {
	        _count++;
	        if (_count > NextFrameAfterCount)
	        {
		        _count = 0;
		        return true;
	        }

	        return false;
        }
    }
}