using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace NeonLib.Graphing {
    public class GenericGraphView : GraphView {
        private UnityEngine.Object target;

        private List<Type> _availableNodeTypes = new List<Type>();
        public virtual List<Type> AvailableNodeTypes {
            get { return _availableNodeTypes; }
        }

        public GenericGraphView() : this(null) {

        }

        public GenericGraphView(UnityEngine.Object target) : base() {
            this.target = target;
            GridBackground background = new GridBackground();
            Insert(0, background);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            var contentZoomer = new ContentZoomer();
            contentZoomer.minScale = 0.1f; // Optional: Set the minimum zoom scale
            contentZoomer.maxScale = 2f;   // Optional: Set the maximum zoom scale
            this.AddManipulator(contentZoomer);
        }
    }
}