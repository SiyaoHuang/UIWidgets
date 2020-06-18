using Unity.UIWidgets.engine;
using UnityEngine;

namespace Unity.UIWidgets.engine.raycast {
    [RequireComponent(typeof(RectTransform))]
    public class UIWidgetsRaycastablePanel : UIWidgetsPanel, ICanvasRaycastFilter {
        int windowHashCode;

        protected override void InitWindowAdapter() {
            base.InitWindowAdapter();
            this.windowHashCode = this.window.GetHashCode();
            RaycastManager.NewWindow(this.windowHashCode);
        }

        protected void OnDisable() {
            base.OnDisable();
            RaycastManager.DisposeWindow(this.windowHashCode);
        }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
            return true;
        }
    }
}