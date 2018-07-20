/* 
 *  Copyright 2008, 2015 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using ZXMAK2.Engine;
using SysFont = System.Drawing.Font;
using D3dFont = Microsoft.DirectX.Direct3D.Font;


namespace ZXMAK2.Host.WinForms.Mdx.Renderers
{
    public class OsdRenderer : RendererBase
    {
        #region Constants

        private const int GraphLength = 150;

        #endregion Constants


        #region Fields

        private readonly GraphMonitor _graphRender = new GraphMonitor(GraphLength);
        private readonly GraphMonitor _graphLoad = new GraphMonitor(GraphLength);
        private readonly GraphMonitor _graphUpdate = new GraphMonitor(GraphLength);
#if SHOW_LATENCY
        private readonly GraphMonitor _graphLatency = new GraphMonitor(GraphLength);
        //private readonly GraphMonitor _copyGraph = new GraphMonitor(GraphLength);
#endif
        private D3dFont _font;

        private bool _isRunning;

        #endregion Fields


        #region .ctor

        public OsdRenderer(AllocatorPresenter allocator)
            : base(allocator)
        {
        }

        #endregion .ctor


        #region Public

        public int FrameStartTact { get; set; }

        public int SampleRate { get; set; }

        public Size FrameSize { get; set; }

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (_isRunning == value)
                {
                    return;
                }
                _isRunning = value;
                _graphUpdate.ResetPeriod();
                //_graphRender.ResetPeriod();
                //_renderGraph.Clear();
                //_loadGraph.Clear();
            }
        }

        public void UpdateFrame(double updateTime)
        {
            if (IsLoaded)
            {
                _graphUpdate.PushPeriod();
                _graphLoad.PushValue(updateTime);
            }
        }

        public void UpdatePresent()
        {
            if (IsLoaded)
            {
                _graphRender.PushPeriod();
            }
        }

        #endregion Public


        #region RendererBase

        protected override void LoadSynchronized()
        {
            base.LoadSynchronized();
            var gdiFont = new SysFont(
                "Microsoft Sans Serif",
                10f/*8.25f*/,
                FontStyle.Bold,
                GraphicsUnit.Pixel);
            _font = new D3dFont(Allocator.Device, gdiFont);
        }

        protected override void UnloadSynchronized()
        {
            base.UnloadSynchronized();
            if (_font != null)
            {
                _font.Dispose();
                _font = null;
            }
        }

        protected override void RenderSynchronized(int width, int height)
        {
            base.RenderSynchronized(width, height);
            var wndSize = new SizeF(width, height);
            var frameRate = Allocator.Device.DisplayMode.RefreshRate;
            var graphRender = _graphRender.Get();
            var graphLoad = _graphLoad.Get();
#if SHOW_LATENCY
            var graphLatency = _graphLatency.Get();
            //var graphCopy = _copyGraph.Get();
#endif
            var graphUpdate = _graphUpdate.Get();
            var frequency = GraphMonitor.Frequency;
            var limitDisplay = frequency / frameRate;
            var limit50 = frequency / 50D;
            var limit1ms = frequency / 1000D;
            var maxRender = graphRender.Max();
            var maxLoad = graphLoad.Max();
            var minT = graphRender.Min() * 1000D / frequency;
            var avgT = graphRender.Average() * 1000D / frequency;
            var maxT = maxRender * 1000D / frequency;
#if SHOW_LATENCY
            var minL = _graphLatency.IsDataAvailable ? graphLatency.Min() * 1000D / frequency : 0D;
            var avgL = _graphLatency.IsDataAvailable ? graphLatency.Average() * 1000D / frequency : 0D;
            var maxL = _graphLatency.IsDataAvailable ? graphLatency.Max() * 1000D / frequency : 0D;
#endif
            var avgE = graphLoad.Average() * 1000D / frequency;
            var avgU = graphUpdate.Average() * 1000D / frequency;
            var maxScale = Math.Max(maxRender, maxLoad);
            maxScale = Math.Max(maxScale, limit50);
            maxScale = Math.Max(maxScale, limitDisplay);
            var fpsRender = 1000D / avgT;
            var fpsUpdate = 1000D / avgU;
            var textValue = string.Format(
                "Render FPS: {0:F3}\n" +
                "Update FPS: {1:F3}\n" +
                "Device FPS: {2}\n" +
                "Back: [{3}, {4}]\n" +
                "Frame: [{5}, {6}]\n" +
                "Sound: {7:F3} kHz\n" +
                "FrameStart: {8}T",
                fpsRender,
                IsRunning ? fpsUpdate : (double?)null,
                frameRate,
                wndSize.Width,
                wndSize.Height,
                FrameSize.Width,
                FrameSize.Height,
                SampleRate / 1000D,
                FrameStartTact);
            var textRect = _font.MeasureString(
                null,
                textValue,
                DrawTextFormat.NoClip,
                Color.Yellow);
            textRect = new Rectangle(
                textRect.Left,
                textRect.Top,
                Math.Max(textRect.Width + 10, GraphLength),
                textRect.Height);
            FillRect(textRect, Color.FromArgb(192, Color.Green));
            _font.DrawText(
                null,
                textValue,
                textRect,
                DrawTextFormat.NoClip,
                Color.Yellow);
            // Draw graphs
            var graphRect = new Rectangle(
                textRect.Left,
                textRect.Top + textRect.Height,
                GraphLength,
                (int)(wndSize.Height - textRect.Top - textRect.Height));
            FillRect(graphRect, Color.FromArgb(192, Color.Black));
            RenderGraph(graphRender, maxScale, graphRect, Color.FromArgb(196, Color.Lime));
            RenderGraph(graphLoad, maxScale, graphRect, Color.FromArgb(196, Color.Red));
            //RenderGraph(graphCopy, maxTime, graphRect, Color.FromArgb(196, Color.Yellow));
            RenderLimit(limitDisplay, maxScale, graphRect, Color.FromArgb(196, Color.Yellow));
            RenderLimit(limit50, maxScale, graphRect, Color.FromArgb(196, Color.Magenta));
            DrawGraphGrid(maxScale, limit1ms, graphRect, _graphRender.GetIndex(), Color.FromArgb(64, Color.White));

            var msgTime = string.Format(
                "MinT: {0:F3} [ms]\nAvgT: {1:F3} [ms]\nMaxT: {2:F3} [ms]\nAvgE: {3:F3} [ms]",
                minT,
                avgT,
                maxT,
                avgE);
#if SHOW_LATENCY
                if (_graphLatency.IsDataAvailable)
                {
                    msgTime = string.Format(
                        "{0}\nMinL: {1:F3} [ms]\nAvgL: {2:F3} [ms]\nMaxL: {3:F3} [ms]",
                        msgTime,
                        minL,
                        avgL,
                        maxL);
                }
#endif
            _font.DrawText(
                null,
                msgTime,
                graphRect,
                DrawTextFormat.NoClip,
                Color.FromArgb(156, Color.Yellow));

        }

        #endregion RendererBase


        #region Private

        private void DrawGraphGrid(
            double maxValue,
            double step,
            Rectangle rect,
            int index,
            Color color)
        {
            var icolor = color.ToArgb();
            var list = new List<Point>();
            var lineCount = maxValue / step;
            if (lineCount > 40 * 40D)
            {
                step = maxValue / 20D;
                icolor = Color.FromArgb(color.A, Color.Violet).ToArgb();
            }
            else if (lineCount > 40D)
            {
                step *= 10D;
                icolor = Color.FromArgb(color.A, Color.Red).ToArgb();
            }
            for (var t = 0D; t < maxValue; t += step)
            {
                var value = (int)((1D - (t / maxValue)) * rect.Height);
                list.Add(new Point(rect.Left, rect.Top + value));
                list.Add(new Point(rect.Left + rect.Width, rect.Top + value));
            }
            for (var t = 0; t < GraphLength; t += 25)
            {
                var ts = GraphLength - (t + index) % GraphLength;
                list.Add(new Point(rect.Left + ts, rect.Top));
                list.Add(new Point(rect.Left + ts, rect.Top + rect.Height));
            }

            var vertices = list
                .Select(p => new CustomVertex.TransformedColored(p.X, p.Y, 0, 1f, icolor))
                .ToArray();
            Allocator.Device.VertexFormat = CustomVertex.TransformedColored.Format | VertexFormats.Diffuse;
            Allocator.Device.DrawUserPrimitives(PrimitiveType.LineList, vertices.Length / 2, vertices);
        }

        private void RenderLimit(
            double limit,
            double maxValue,
            Rectangle rect,
            Color color)
        {
            if (limit < 0 || limit > maxValue)
            {
                return;
            }
            var icolor = color.ToArgb();
            var list = new List<Point>();
            var value = 1D - (limit / maxValue);
            if (value < 0D)
            {
                value = 0;
            }
            var hValue = (int)(value * rect.Height);
            list.Add(new Point(rect.Left, rect.Top + hValue));
            list.Add(new Point(rect.Left + rect.Width, rect.Top + hValue));
            var vertices = list
                .Select(p => new CustomVertex.TransformedColored(p.X, p.Y, 0, 1f, icolor))
                .ToArray();
            Allocator.Device.VertexFormat = CustomVertex.TransformedColored.Format | VertexFormats.Diffuse;
            Allocator.Device.DrawUserPrimitives(PrimitiveType.LineList, vertices.Length / 2, vertices);
        }

        private void RenderGraph(
            double[] graph,
            double maxValue,
            Rectangle rect,
            Color color)
        {
            if (graph.Length < 1)
            {
                return;
            }
            var icolor = color.ToArgb();
            var list = new List<Point>();
            for (var x = 0; x < graph.Length && x < rect.Width; x++)
            {
                var value = 1D - (graph[x] / maxValue);
                if (value < 0D)
                {
                    value = 0;
                }
                var hValue = (int)(value * rect.Height);
                list.Add(new Point(rect.Left + x, rect.Top + rect.Height));
                list.Add(new Point(rect.Left + x, rect.Top + hValue));
            }
            var vertices = list
                .Select(p => new CustomVertex.TransformedColored(p.X, p.Y, 0, 1f, icolor))
                .ToArray();
            Allocator.Device.VertexFormat = CustomVertex.TransformedColored.Format | VertexFormats.Diffuse;
            Allocator.Device.DrawUserPrimitives(PrimitiveType.LineList, vertices.Length / 2, vertices);
        }

        private void FillRect(Rectangle rect, Color color)
        {
            var icolor = color.ToArgb();
            var rectv = new[]
            {
                new CustomVertex.TransformedColored(rect.Left, rect.Top+rect.Height+0.5F, 0, 1f, icolor),
                new CustomVertex.TransformedColored(rect.Left, rect.Top, 0, 1f, icolor),
                new CustomVertex.TransformedColored(rect.Left+rect.Width, rect.Top+rect.Height+0.5F, 0, 1f, icolor),
                new CustomVertex.TransformedColored(rect.Left+rect.Width, rect.Top, 0, 1f, icolor),
            };
            Allocator.Device.VertexFormat = CustomVertex.TransformedColored.Format | VertexFormats.Diffuse;
            Allocator.Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, rectv);
        }

        #endregion Private
    }
}
