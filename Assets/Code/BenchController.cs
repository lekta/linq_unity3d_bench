using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace LinqBenchmark {
    public class BenchController : MonoBehaviour {

        private Stopwatch _sw;
        private Transform _transform;


        private void Awake() {
            _transform = transform;
        }

        private void Start() {
            Debug.Log("Run Linq bench...");

            int iterations = 100_000;
            int innerIterations = 100;

            int counter = 0;
            var array = Enumerable.Range(0, innerIterations).ToArray();
            var list = array.ToList();
            var enumerable = (IEnumerable<int>)list;

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) {
                for (int j = 0; j < innerIterations; j++) {
                    MoveOperation();
                }
            }
            Debug.Log($"Move Operations for: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) {
                for (int j = 0; j < innerIterations; j++) {
                    counter += array[j];
                }
            }
            Debug.Log($"Math Operations for, array: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) {
                for (int j = 0; j < innerIterations; j++) {
                    counter += list[j];
                }
            }
            Debug.Log($"Math Operations for, list: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) {
                foreach (var value in array) {
                    counter += value;
                }
            }
            Debug.Log($"Math Operations foreach, array: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) {
                foreach (var value in list) {
                    counter += value;
                }
            }
            Debug.Log($"Math Operations foreach, list: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) {
                foreach (var value in enumerable) {
                    counter += value;
                }
            }
            Debug.Log($"Math Operations foreach, enumerable: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) {
                counter += array.Sum();
            }
            Debug.Log($"Math Operations linq, array: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) {
                counter += list.Sum();
            }
            Debug.Log($"Math Operations linq, list: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) {
                counter += enumerable.Sum();
            }
            Debug.Log($"Math Operations linq, enumerable: <b>{_sw.ElapsedMilliseconds}</b> ms");

            Debug.Log($"Complete Linq bench, counter {counter}\n");
        }

        private void MoveOperation() {
            _transform.localPosition = new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), Random.Range(-200, 200));
        }

    }
}