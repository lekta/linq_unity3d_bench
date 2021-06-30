using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UnityBench {
    public class MathBenchController : MonoBehaviour {

        [SerializeField] private int _iterations = 10_000;
        [SerializeField] private int _operations = 100;
        private int _chk;

        private Stopwatch _sw;
        private Transform _transform;


        private void Awake() {
            _transform = transform;

            Debug.Log($"Bench prepared");
            Debug.Log(" \n");
        }

    }
}