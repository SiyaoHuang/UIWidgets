using System.Collections.Generic;
using Unity.UIWidgets.editor;
using Unity.UIWidgets.external.simplejson;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using Texture = UnityEngine.Texture;

namespace Unity.UIWidgets.engine {
    public class UIWidgetWindowAdapter : WindowAdapter {
        readonly UIWidgetsPanel _uiWidgetsPanel;
        bool _needsPaint;


        protected override void updateSafeArea() {
            this._padding = this._uiWidgetsPanel.viewPadding;
            this._viewInsets = this._uiWidgetsPanel.viewInsets;
        }

        protected override bool hasFocus() {
            return EventSystem.current != null;
        }

        public override void scheduleFrame(bool regenerateLayerTree = true) {
            base.scheduleFrame(regenerateLayerTree);
            this._needsPaint = true;
        }

        public UIWidgetWindowAdapter(UIWidgetsPanel uiWidgetsPanel) {
            this._uiWidgetsPanel = uiWidgetsPanel;
        }


        public override void OnGUI(Event evt) {
            if (this.displayMetricsChanged()) {
                this._needsPaint = true;
            }

            if (evt.type == EventType.Repaint) {
                if (!this._needsPaint) {
                    return;
                }

                this._needsPaint = false;
            }

            base.OnGUI(evt);
        }



        protected override float queryDevicePixelRatio() {
            return this._uiWidgetsPanel.devicePixelRatio;
        }
        
        protected override int queryAntiAliasing() {
            return this._uiWidgetsPanel.antiAliasing;
        }

        protected override Vector2 queryWindowSize() {
            return new Vector2(0, 0);
        }

       
    }

    [RequireComponent(typeof(RectTransform))]
    public class UIWidgetsPanel : WindowHost {
        static Event _repaintEvent;

        [Tooltip("set to zero if you want to use the default device pixel ratio of the target platforms; otherwise the " +
                 "device pixel ratio will be forced to the given value on all devices.")]
        [SerializeField] protected float devicePixelRatioOverride;
        
        [Tooltip("set to true will enable the hardware anti-alias feature, which will improve the appearance of the UI greatly but " +
                 "making it much slower. Enable it only when seriously required.")]
        [SerializeField] protected bool hardwareAntiAliasing = false;
        WindowAdapter _windowAdapter;
        Texture _texture;
        Vector2 _lastMouseMove;

        readonly HashSet<int> _enteredPointers = new HashSet<int>();

        bool _viewMetricsCallbackRegistered;

        bool _mouseEntered {
            get { return !this._enteredPointers.isEmpty(); }
        }

        DisplayMetrics _displayMetrics;

        const int mouseButtonNum = 3;

        void _handleViewMetricsChanged(string method, List<JSONNode> args) {
            this._windowAdapter.onViewMetricsChanged();
            this._displayMetrics.onViewMetricsChanged();
        }

        protected virtual void InitWindowAdapter() {
            D.assert(this._windowAdapter == null);
            this._windowAdapter = new UIWidgetWindowAdapter(this);

            this._windowAdapter.OnEnable();
        }

        protected void OnEnable() {

            //Disable the default touch -> mouse event conversion on mobile devices
            Input.simulateMouseWithTouches = false;

            this._displayMetrics = DisplayMetricsProvider.provider();
            this._displayMetrics.OnEnable();

            this._enteredPointers.Clear();

            if (_repaintEvent == null) {
                _repaintEvent = new Event {type = EventType.Repaint};
            }

            this.InitWindowAdapter();

            Widget root;
            using (this._windowAdapter.getScope()) {
                root = this.createWidget();
            }

            this._windowAdapter.attachRootWidget(root);
            this._lastMouseMove = Input.mousePosition;
        }

        public float devicePixelRatio {
            get {
                return this.devicePixelRatioOverride > 0
                    ? this.devicePixelRatioOverride
                    : this._displayMetrics.devicePixelRatio;
            }
        }
        
        public int antiAliasing {
            get { return this.hardwareAntiAliasing ? Window.defaultAntiAliasing : 0; }
        }

        public WindowPadding viewPadding {
            get { return this._displayMetrics.viewPadding; }
        }

        public WindowPadding viewInsets {
            get { return this._displayMetrics.viewInsets; }
        }

        protected void OnDisable() {
            D.assert(this._windowAdapter != null);
            this._windowAdapter.OnDisable();
            this._windowAdapter = null;
        }

        protected virtual Widget createWidget() {
            return null;
        }

        public void recreateWidget() {
            Widget root;
            using (this._windowAdapter.getScope()) {
                root = this.createWidget();
            }

            this._windowAdapter.attachRootWidget(root);
        }

        public virtual void Update(bool showRoot) {
            this._displayMetrics.Update();
            UIWidgetsMessageManager.ensureUIWidgetsMessageManagerIfNeeded();

#if UNITY_ANDROID
            if (Input.GetKeyDown(KeyCode.Escape)) {
                this._windowAdapter.withBinding(() => { WidgetsBinding.instance.handlePopRoute(); });
            }
#endif

            if (!this._viewMetricsCallbackRegistered) {
                this._viewMetricsCallbackRegistered = true;
                UIWidgetsMessageManager.instance?.AddChannelMessageDelegate("ViewportMatricsChanged",
                    this._handleViewMetricsChanged);
            }


            this._lastMouseMove = Input.mousePosition;
            

            D.assert(this._windowAdapter != null);
            this._windowAdapter.Update();
            this._windowAdapter.OnGUI(_repaintEvent);
            
            if (showRoot) {
                var node =
                    this._windowAdapter.widgetInspectorService.widgetsBinding.renderViewElement.toDiagnosticsNode();
                Debug.Log(node.toStringDeep());
            }
        }
     
        public Window window {
            get { return this._windowAdapter; }
        }
    }
}