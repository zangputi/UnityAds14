using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Common
{
    public class MoveTo
    {
        Vector3 _endPos;
        Vector3 _startPos;
        float _moveTime;
        Action _complete;
        bool _isMove = false;
        float _passTime;

        Vector3 _curPos;

        public void OnUpdate(float dt)
        {
            if (!_isMove)
            {
                return;
            }
            var dis = _endPos - _startPos;

            _passTime += dt;

            bool isComplete = false;

            if (_passTime >= _moveTime)
            {
                _passTime = _moveTime;
                isComplete = true;
            }

            float rate = _passTime / _moveTime;
            var moveDis = dis * rate;

            if (!isComplete)
            {
                _curPos = _startPos + moveDis;
            }
            else
            {
                //移动完成
                _curPos = _endPos;
                _isMove = false;

                _complete?.Invoke();
            }
        }

        public void Move(Vector3 start, Vector3 end, float time, Action complete = null)
        {
            _isMove = true;
            _startPos = start;
            _curPos = start;
            _endPos = end;
            _moveTime = time;
            _complete = complete;
            _passTime = 0;
        }

        public Vector3 CurPos { get => _curPos; }

        public bool IsMove { get => _isMove; }
    }
}

