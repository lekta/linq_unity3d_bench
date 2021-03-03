using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace LinqBenchmark {
    public class BenchController : MonoBehaviour {

        [SerializeField] private int _iterations = 10_000;
        [SerializeField] private int _operations = 100;
        private int _chk;

        private Stopwatch _sw;
        private Transform _transform;

        private int[] _array;
        private List<int> _list;
        private List<int> _heavyList;
        private IEnumerable<int> _enumerable;

        private LinqAnalogs _linqAnalogs = new LinqAnalogs();


        private void Awake() {
            _transform = transform;

            _array = Enumerable.Range(0, _operations).ToArray();
            _list = _array.ToList();
            _heavyList = Enumerable.Range(0, _operations * _iterations).ToList();
            _enumerable = _list;

            _linqAnalogs.Setup(_iterations, _operations);

            Debug.Log($"Bench prepared");
            Debug.Log(" \n");
        }


        public void Run_LinearBench() {
            Debug.Log($"  Simple linear bench, {GetIterationsInfo()}");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                var displacement = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));

                for (int j = 0; j < _operations; j++) {
                    _transform.Translate(displacement);
                }
            }
            Debug.Log($"Transform Translate Operations for: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                var displacement = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
                
                for (int j = 0; j < _operations; j++) {
                    _transform.localPosition += displacement;
                }
            }
            Debug.Log($"Transform move Operations for: <b>{_sw.ElapsedMilliseconds}</b> ms");

            var transformList = Enumerable.Repeat(transform, _operations).ToList();
            
            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                var displacement = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));

                _chk += transformList.Select(t => t.position += displacement).Count();
            }
            Debug.Log($"Transform move Operations linq: <b>{_sw.ElapsedMilliseconds}</b> ms");
            

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                for (int j = 0; j < _operations; j++) {
                    _chk += _array[j];
                }
            }
            Debug.Log($"Math Operations for, array: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                for (int j = 0; j < _operations; j++) {
                    _chk += _list[j];
                }
            }
            Debug.Log($"Math Operations for, list: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                foreach (var value in _array) {
                    _chk += value;
                }
            }
            Debug.Log($"Math Operations foreach, array: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                foreach (var value in _list) {
                    _chk += value;
                }
            }
            Debug.Log($"Math Operations foreach, list: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                foreach (var value in _enumerable) {
                    _chk += value;
                }
            }
            Debug.Log($"Math Operations foreach, enumerable: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                _chk += _array.Sum();
            }
            Debug.Log($"Math Operations linq, array: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                _chk += _list.Sum();
            }
            Debug.Log($"Math Operations linq, list: <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                _chk += _enumerable.Sum();
            }
            Debug.Log($"Math Operations linq, enumerable: <b>{_sw.ElapsedMilliseconds}</b> ms");
            Debug.Log(" \n");
        }

        public void Run_LambdaBench() {
            Debug.Log($"  Simple lambda bench, {GetIterationsInfo()}");

            var array = Enumerable.Range(0, _operations).ToArray();
            var list = array.ToList();
            var enumerable = (IEnumerable<int>)list;

            ForOperation("Move Operations", _ => { _transform.localPosition += Vector3.zero; return 0; });

            ForOperation("Math Operations array", j => array[j]);
            ForOperation("Math Operations list", j => list[j]);

            ForeachOperation("Math Operations array", array);
            ForeachOperation("Math Operations list", list);
            ForeachOperation("Math Operations enumerable", enumerable);
            
            SingleOperation("Math Operations linq, array", () => array.Sum());
            SingleOperation("Math Operations linq, list", () => list.Sum());
            SingleOperation("Math Operations linq, enumerable", () => enumerable.Sum());

            Debug.Log(" \n");
        }

        public void Run_SingleSilentFor() {
            for (int i = 0; i < _iterations; i++) {
                for (int j = 0; j < _operations; j++) {
                    _chk += 1;
                }
            }
        }

        public void Run_SingleSilentForeach() {
            for (int i = 0; i < _iterations; i++) {
                foreach (var value in _list) {
                    _chk += value;
                }
            }
        }
        
        public void Run_SingleSilentLinq() {
            for (int i = 0; i < _iterations; i++) {
                _chk += _list.Sum();
            }
        }

        public void Run_SingleHeavyLinq() {
            _sw = Stopwatch.StartNew();
            
            _chk += _heavyList
               .Select(v => v - 1)
               .Reverse()
               .Where(v => v % 2 == 0)
               .Distinct()
               .Aggregate(0, (s, v) => s + v);

            Debug.Log($"RunSingleHeavyLinq, {_chk}: <b>{_sw.ElapsedMilliseconds}</b> ms");
        }

        public void Run_SingleHeavyLinqParallel() {
            _sw = Stopwatch.StartNew();
            
            _chk += _heavyList
               .AsParallel()
               .Select(v => v - 1)
               .Reverse()
               .Where(v => v % 2 == 0)
               .Distinct()
               .Aggregate(0, (s, v) => s + v);
            
            Debug.Log($"RunSingleHeavyLinqParallel, {_chk}: <b>{_sw.ElapsedMilliseconds}</b> ms");
        }
        
        public void RunAnalog_FirstOrDefault() => _linqAnalogs.Compare_FirstOrDefault();

        public void RunAnalog_Any() => _linqAnalogs.Compare_Any();

        public void RunAnalog_Where() => _linqAnalogs.Compare_Where();


        private void ForOperation(string desc, Func<int, int> operation) {
            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                for (int j = 0; j < _operations; j++) {
                    _chk += operation(j);
                }
            }
            Debug.Log($"{desc}, for: <b>{_sw.ElapsedMilliseconds}</b> ms");
        }

        private void ForeachOperation<T>(string desc, T enumerable) where T: IEnumerable<int> {
            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                foreach (var value in enumerable) {
                    _chk += value;
                }
            }
            Debug.Log($"{desc}, foreach: <b>{_sw.ElapsedMilliseconds}</b> ms");
        }

        private void SingleOperation(string desc, Func<int> operation) {
            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                _chk += operation();
            }
            Debug.Log($"{desc}: <b>{_sw.ElapsedMilliseconds}</b> ms");
        }

        private string GetIterationsInfo() {
            float iterLn = Mathf.Log10(_iterations * _operations);
            return $"iterations: {_iterations}, operations in iteration: {_operations}, total operation: 10^{iterLn:0.#}";
        }

    }
}