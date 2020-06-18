using System.Collections.Generic;

namespace Unity.UIWidgets.InternalBridge {
    public static class NoAllocHelpersBridge<T> {
        public static T[] ExtractArrayFromListT(List<T> list) {
            // return UnityEngine.NoAllocHelpers.ExtractArrayFromListT(list);
            return extractArrayFromListT(list);
        }

        static T[] extractArrayFromListT(List<T> list) {
            int size = list.Count;
            T[] ans = new T[size];
            int i = 0;
            foreach (var VARIABLE in list) {
                ans[i] = VARIABLE;
                i++;
            }

            return ans;
        }

        public static void ResizeList(List<T> list, int size) {
            if (size < list.Count) {
                for (int i = 0; i < list.Count - size; i++) {
                    list.RemoveAt(size);
                }
                return;
            }

            if (size == list.Count) {
                return;
            }

            if (list.Capacity < size) {
                list.Capacity = size;
            }
        }

        public static void EnsureListElemCount(List<T> list, int size) {
            list.Clear();
            if (list.Capacity < size) {
                list.Capacity = size;
            }

            ResizeList(list, size);
        }
    }
}
