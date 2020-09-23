using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Docnet.Core.Bindings
{
    public unsafe class PdfBitmap : IDisposable
    {
        private readonly FpdfBitmapT _pdfBitmap;

        public PixelFormat Type => PixelFormat.Format32bppPArgb;

        public int Width { get; }

        public int Height { get; }

        public int Stride { get; }

        public IntPtr Scan0 { get; }

        public Bitmap Bitmap { get; }


        public bool IsDisposed { get; private set; }

        internal PdfBitmap(FpdfBitmapT pdfBitmap, int width, int height)
        {
            _pdfBitmap = pdfBitmap;
            Scan0 = fpdf_view.FPDFBitmapGetBuffer(pdfBitmap);
            Stride = fpdf_view.FPDFBitmapGetStride(pdfBitmap);
            Height = height;
            Width = width;
            Bitmap = new Bitmap(width, height, Stride, PixelFormat.Format32bppPArgb, Scan0);
        }

        public Stream GetStream()
        {
            var pointer = (byte*) Scan0;
            if (pointer == null)
                return null;

            return new UnmanagedMemoryStream(pointer, Stride * Height);
        }

        private void ReleaseUnmanagedResources()
        {
            fpdf_view.FPDFBitmapDestroy(_pdfBitmap);
            // TODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                Bitmap?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PdfBitmap()
        {
            Dispose(false);
        }
    }
}