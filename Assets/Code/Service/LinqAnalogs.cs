using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace UnityBench {
    public class LinqAnalogs {

        private int _iterations;
        private int _operations;
        private Stopwatch _sw;

        private List<int> _list;


        public void Setup(int iterations, int operations) {
            _iterations = iterations;
            _operations = operations;

            _list = Enumerable.Range(0, _operations).ToList();
        }

        public void Compare_FirstOrDefault() {
            int target = _operations / 2;
            int result = 0;

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                for (int j = 0; j < _operations; j++) {
                    if (_list[j] == target) {
                        result = target;
                        break;
                    }
                }
            }
            Debug.Log($"FirstOrDefault, for: {result}, <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                result = _list.FirstOrDefault(v => v == target);
            }
            Debug.Log($"FirstOrDefault, linq: {result}, <b>{_sw.ElapsedMilliseconds}</b> ms");
        }

        public void Compare_Any() {
            int target = _operations / 2;
            bool result = false;

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                for (int j = 0; j < _operations; j++) {
                    if (_list[j] == target) {
                        result = true;
                        break;
                    }
                }
            }
            Debug.Log($"FirstOrDefault, for: {result}, <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                result = _list.Any(v => v == target);
            }
            Debug.Log($"FirstOrDefault, linq: {result}, <b>{_sw.ElapsedMilliseconds}</b> ms");
        }


        public void Compare_Where() {
            var output = new List<int>();

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                output = new List<int>();

                for (int j = 0; j < _operations; j++) {
                    int v = _list[j];

                    if (v % 3 == 0) {
                        output.Add(v);
                        break;
                    }
                }
            }
            Debug.Log($"FirstOrDefault, for: {output.Count}, <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                output = _list.Where(v => v % 3 == 0).ToList();
            }
            Debug.Log($"FirstOrDefault, linq: {output.Count}, <b>{_sw.ElapsedMilliseconds}</b> ms");
        }

        public void Compare_GroupBy() {
            int keys = 5;
            var output = Enumerable.Range(0, keys).ToDictionary(i => i, _ => new List<int>());
            
            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                for (int j = 0; j < output.Count; j++) {
                    output[j].Clear();
                }

                for (int j = 0; j < _operations; j++) {
                    int v = _list[j];

                    int gIdx = v % keys;
                    output[gIdx].Add(v);
                }
            }
            Debug.Log($"FirstOrDefault, for: {output.Count}, <b>{_sw.ElapsedMilliseconds}</b> ms");

            _sw = Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++) {
                output = _list.GroupBy(v => v % keys).ToDictionary(g => g.Key, g => g.ToList());
            }
            Debug.Log($"FirstOrDefault, linq: {output.Count}, <b>{_sw.ElapsedMilliseconds}</b> ms");
            
            
        } 
        
    }
}