using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class CallLater
    {
        List<CallData> _list = new List<CallData>();

        public void OnUpdate(float dt)
        {
            List<CallData> callList = new List<CallData>();

            for(int i = 0;i< _list.Count;i++)
            {
                var data = _list[i];
                data.time -= dt;

                if (data.time <= 0)
                {
                    callList.Add(data);
                }
            }

            for (int i = 0; i < callList.Count; i++)
            {
                var data = callList[i];
                data.call?.Invoke();

                _list.Remove(data);
            }
        }

        public void Add(float time, Action call)
        {
            var data = new CallData();
            data.time = time;
            data.call = call;
            _list.Add(data);
        }
    }


    public class CallData
    {
        public Action call;
        public float time;
    }
}

