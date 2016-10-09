﻿using System;
using SharpDX.Mathematics.Interop;
using WinApi.Core;
using WinApi.DxUtils.D3D11;
using WinApi.Gdi32;
using WinApi.User32;

namespace Sample.DirectX.Contexts
{
    public class D3DGraphicsContext : IGraphicsContext
    {
        private D3DMetaResource m_d3DMetaResource;
        private IntPtr m_hwnd;
        private Size m_size;

        public void Init(IntPtr hwnd, ref Size size, bool deferInitUntilFirstDraw = true)
        {
            this.m_hwnd = hwnd;
            this.m_size = size;
            if (!deferInitUntilFirstDraw) EnsureDxResources();
        }

        public void Draw()
        {
            EnsureDxResources();
            var target = m_d3DMetaResource.RenderTargetView;
            var context = m_d3DMetaResource.Context;
            var swapChain = m_d3DMetaResource.SwapChain;

            context.ClearRenderTargetView(target, new RawColor4(0.5f, 0.6f, 0.7f, 0.7f));
            swapChain.Present(1, 0);
        }

        public void Resize(ref Size size)
        {
            m_size = size;
            m_d3DMetaResource?.Resize(ref m_size);
        }

        private void EnsureDxResources()
        {
            if (m_d3DMetaResource == null)
            {
                PaintDefault();
                m_d3DMetaResource = D3DMetaFactory.Create();
                m_d3DMetaResource.Initalize(m_hwnd, m_size);
            }
        }

        private void PaintDefault()
        {
            PaintStruct ps;
            var hdc = User32Methods.BeginPaint(m_hwnd, out ps);
            var b = Gdi32Methods.CreateSolidBrush(0);
            User32Methods.FillRect(hdc, ref ps.PaintRect, b);
            Gdi32Methods.DeleteObject(b);
            User32Methods.EndPaint(m_hwnd, ref ps);
        }

        public void Dispose()
        {
            m_d3DMetaResource.Dispose();
        }
    }
}
