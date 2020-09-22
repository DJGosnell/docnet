using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Docnet.Core.Bindings
{
    public class PdfBitmap : IDisposable
    {
        private readonly FpdfBitmapT _pdfBitmap;
        private readonly IntPtr _buffer;
        public PixelFormat Type => PixelFormat.Format32bppPArgb;

        public int Width { get; set; }

        public int Height { get; set; }

        public int Stride { get; set; }

        public IntPtr Scan0 { get; set; }

        public Bitmap Bitmap { get; }

        public bool IsDisposed { get; private set; }

        internal PdfBitmap(FpdfBitmapT pdfBitmap, int width, int height)
        {
            _pdfBitmap = pdfBitmap;
            var buffer = fpdf_view.FPDFBitmapGetBuffer(pdfBitmap);
            var stride = fpdf_view.FPDFBitmapGetStride(pdfBitmap);
            Bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppPArgb, buffer);
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
