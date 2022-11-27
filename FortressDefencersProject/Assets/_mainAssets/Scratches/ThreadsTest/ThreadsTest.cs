using DevourDev.MonoExtentions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FD.Test
{
    public class ThreadsTest : MonoBehaviour
    {
        [SerializeField] private Button _b1;
        [SerializeField] private Button _b2;
        [SerializeField] private Button _b3;
        [SerializeField] private Button _b4;

        [SerializeField] private Image _img;

        private bool _up;

        private readonly object _locker = new();

        private void Start()
        {
            _b1.onClick.AddListener(HandleB1);
            _b2.onClick.AddListener(HandleB2);
            _b3.onClick.AddListener(HandleB3);
            _b4.onClick.AddListener(HandleB4);
        }

        private void OnDestroy()
        {
            _b1.onClick.RemoveListener(HandleB1);
            _b2.onClick.RemoveListener(HandleB2);
            _b3.onClick.RemoveListener(HandleB3);
            _b4.onClick.RemoveListener(HandleB4);
        }

        private void Update()
        {
            Color c = _img.color;
            var delta = Color.white * Time.deltaTime;
            if (_up)
            {
                c += delta;

                if (c.r > 0.9f)
                    _up = false;
            }
            else
            {
                c -= delta;
                if (c.r < 0.1f)
                    _up = true;
            }
            c.a = 1;
            _img.color = c;
        }

        private async void HandleB1()
        {

            Debug.Log("b1 started");
            //Monitor.Enter(_locker);
           await HeavyTask();
            Debug.Log("b1 finished");
        }

        private async void HandleB2()
        {
            Debug.Log("b2 started");
            //Monitor.Enter(_locker);
            await HeavyTask();
            Debug.Log("b2 finished");
        }

        private async void HandleB3()
        {
            Debug.Log("b3 started");
            //Monitor.Enter(_locker);
            await HeavyTask();
            Debug.Log("b3 finished");
        }

        private async void HandleB4()
        {
            Debug.Log("b4 started");
            //Monitor.Enter(_locker);
            await HeavyTask();
            Debug.Log("b4    finished");
        }

        private async Task HeavyTask()
        {
            this.InvokeOnMainThread(() =>
            {
                Debug.Log("started");
            });
            await Task.Run(HeavyMethod);
            this.InvokeOnMainThread(() =>
            {
                Debug.Log("finished");
            });
        }


        private void HeavyMethod()
        {
            lock (_locker)
            {
                for (int i = 0; i < 100_000_000; i++)
                {
                    var cbrt = Math.Cbrt(i);
                    cbrt++;
                }
            }
          
        }
    }
}
