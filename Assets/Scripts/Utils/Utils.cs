using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Utils {

    public static class Has {
        private static Dictionary<string, bool> storedBools = new Dictionary<string, bool>();
        private static Dictionary<string, float> storedFloats = new Dictionary<string, float>();
        private static Dictionary<string, int> storedInts = new Dictionary<string, int>();

        public static bool Changed(string key, bool boolean, bool ignoreCreationEntry = false) {
            if (storedBools.ContainsKey(key)) {
                if (storedBools[key] != boolean) {
                    storedBools[key] = boolean;
                    return true;
                }
                else
                    return false;
            }
            else {
                storedBools.Add(key, boolean);

                if (ignoreCreationEntry)
                    return false;
                else
                    return true;
            }
        }

        public static bool Changed(string key, float number, bool ignoreCreationEntry = false) {
            if (storedFloats.ContainsKey(key)) {
                if (storedFloats[key] != number) {
                    storedFloats[key] = number;
                    return true;
                }
                else
                    return false;
            }
            else {
                storedFloats.Add(key, number);

                if (ignoreCreationEntry)
                    return false;
                else
                    return true;
            }
        }

        public static bool Changed(string key, int number, bool ignoreCreationEntry = false) {
            if (storedInts.ContainsKey(key)) {
                if (storedInts[key] != number) {
                    storedInts[key] = number;
                    return true;
                }
                else
                    return false;
            }
            else {
                storedInts.Add(key, number);

                if (ignoreCreationEntry)
                    return false;
                else
                    return true;
            }
        }
    }

    public static class Coroutines {

        public static void DoAfter(Action callback, float durationInSeconds, MonoBehaviour owner) {
            owner.StartCoroutine(WaitSecondsWithCallback(callback, durationInSeconds));
        }

        private static IEnumerator WaitSecondsWithCallback(Action action, float duration) {
            yield return new WaitForSeconds(duration);
            action();
        }
    }
}