﻿using GraphX.Controls;
using GraphX.Controls.Models;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphPAD.GraphData.Model
{
    public class EditorObjectManager : IDisposable
    {
        private GraphZone _graphArea;
        private ZoomControl _zoomControl;
        private EdgeBlueprint _edgeBp;
        private ResourceDictionary _rd;

        public EditorObjectManager(GraphZone graphArea, ZoomControl zc)
        {
            _graphArea = graphArea;
            _zoomControl = zc;
            _zoomControl.MouseMove += _zoomControl_MouseMove;
            _rd = new ResourceDictionary
            {
                Source = new Uri("/GraphPad;component/GraphData/Templates/GraphXTemplates.xaml",
                    UriKind.RelativeOrAbsolute)
            };
        }

        public void CreateVirtualEdge(VertexControl source, Point mousePos)
        {
            _edgeBp = new EdgeBlueprint(source, mousePos, (LinearGradientBrush)_rd["EdgeBrush"]);
            _graphArea.InsertCustomChildControl(0, _edgeBp.EdgePath);
        }

        private void _zoomControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_edgeBp == null) return;
            var pos = _zoomControl.TranslatePoint(e.GetPosition(_zoomControl), _graphArea);
            pos.Offset(2, 2);
            _edgeBp.UpdateTargetPosition(pos);
        }

        private void ClearEdgeBp()
        {
            if (_edgeBp == null) return;
            _graphArea.RemoveCustomChildControl(_edgeBp.EdgePath);
            _edgeBp.Dispose();
            _edgeBp = null;
        }

        public void Dispose()
        {
            ClearEdgeBp();
            _graphArea = null;
            if (_zoomControl != null)
                _zoomControl.MouseMove -= _zoomControl_MouseMove;
            _zoomControl = null;
            _rd = null;
        }

        public void DestroyVirtualEdge()
        {
            ClearEdgeBp();
        }
    }

    public class EdgeBlueprint : IDisposable
    {
        public VertexControl Source { get; set; }
        public Point TargetPos { get; set; }
        public Path EdgePath { get; set; }

        public EdgeBlueprint(VertexControl source, Point targetPos, Brush brush)
        {
            EdgePath = new Path() { Stroke = brush, Data = new LineGeometry() };
            Source = source;
            Source.PositionChanged += Source_PositionChanged;
        }

        private void Source_PositionChanged(object sender, VertexPositionEventArgs args)
        {
            UpdateGeometry(Source.GetCenterPosition(), TargetPos);
        }

        internal void UpdateTargetPosition(Point point)
        {
            TargetPos = point;
            UpdateGeometry(Source.GetCenterPosition(), point);
        }

        private void UpdateGeometry(Point start, Point end)
        {
            EdgePath.Data = new LineGeometry(start, end);
            ((LineGeometry)EdgePath.Data).Freeze();
        }

        public void Dispose()
        {
            Source.PositionChanged -= Source_PositionChanged;
            Source = null;
        }

    }
}
