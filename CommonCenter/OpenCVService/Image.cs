using System;
using OpenCvSharp;

namespace OpenCVService
{
    public class Image:IDisposable
    {
        private Mat _original;
        private Mat _matSource;
        private bool isDispose;

        #region Public Properties
        private int _width;

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }


        #endregion

        #region 缩放、灰度、边缘、图形展示
        public Image(string imgPath)
        {
            _matSource = new Mat(imgPath, ImreadModes.Color);
            if (_matSource is null)
                throw new ArgumentNullException("Mat init failed");

            _original = new Mat();
            _matSource.CopyTo(_original);
            this._height = _matSource.Height;
            this._width = _matSource.Width;
        }

        public void ReSize(Size size)
        {
            _matSource = _matSource.Resize(size);
        }

        public void Gray()
        {
            Cv2.CvtColor(_matSource, _matSource, ColorConversionCodes.BGR2GRAY);
        }

        public byte[] Buffer()
        {
            using var ms = new System.IO.MemoryStream();
            _matSource.WriteToStream(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// 查找边缘
        /// </summary>
        /// <param name="threshold1">第一个阈值</param>
        /// <param name="threshold2">第二个阈值</param>
        /// <param name="apertureSize">操作符的光圈大小</param>
        /// <param name="isUsingL2">是否应使用更精确的L2范数来计算图像</param>
        public void Canny(int threshold1=50, int threshold2=200, int apertureSize=3, bool isUsingL2=false)
        {
            Cv2.Canny(_matSource, _matSource, threshold1, threshold2, apertureSize, isUsingL2);
        }

        public void Show(ShowType showType = ShowType.Changed, bool isWaitKey = true)
        {
            switch (showType)
            {
                case ShowType.Original:
                    using (new Window("Original", _original))
                    {
                        if (isWaitKey)
                        {
                            Cv2.WaitKey(0);
                        }
                    }
                    break;
                case ShowType.Changed:
                    using (new Window("Changed", _matSource))
                    {
                        if (isWaitKey)
                        {
                            Cv2.WaitKey(0);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Face
        public Rect[] Face(bool useGray=false, bool isAutoDrawingRect=false)
        {
            Mat newSource = new Mat();
            if (useGray)
            {
                Gray();
                Cv2.EqualizeHist(_matSource, _matSource);
                newSource = _matSource;
            }
            else
            {
                Cv2.CvtColor(_matSource, newSource, ColorConversionCodes.BGR2GRAY);
                Cv2.EqualizeHist(newSource, newSource);
            }

            var path = @"D:\Personal Repository\Common\CommonCenter\OpenCVService\Models\haarcascade_frontalface_alt.xml";
            CascadeClassifier cascade = new CascadeClassifier(path);
            Rect[] faceRects = cascade.DetectMultiScale(
                    image: newSource,
                    scaleFactor: 1.1,
                    minNeighbors:1,
                    flags: HaarDetectionType.DoRoughSearch| HaarDetectionType.ScaleImage,
                    minSize: new Size(30,30)
                );
            if(faceRects.Length <= 0)
            {
                //no face
                return new Rect[0];
            }
            else
            {
                if (isAutoDrawingRect)
                {
                    foreach (var rect in faceRects)
                    {
                        var r = new Rect
                        {
                            X = rect.X + (int)Math.Round(rect.Width * 0.1),
                            Y = rect.Y + (int)Math.Round(rect.Height * 0.1),
                            Width = (int)Math.Round(rect.Width * 0.8),
                            Height = (int)Math.Round(rect.Height * 0.8)
                        };
                        _matSource.Rectangle(r.TopLeft, r.BottomRight, Scalar.Red, 1);
                    }
                    using (new Window("Result", _matSource))
                    {
                        Cv2.WaitKey(0);
                    }
                }
                return faceRects;
            }
        }
        #endregion

        #region 特效

        /// <summary>
        /// 边缘保护滤波
        /// </summary>
        public void EdgePreservingFilter(EdgePreservingMethods methodType= EdgePreservingMethods.NormconvFilter,float sigmaS=60,float sigmaR=0.45f)
        {
            Cv2.EdgePreservingFilter(_matSource, _matSource, methodType, sigmaS, sigmaR);
        }

        /// <summary>
        /// 细节增强
        /// </summary>
        public void DetailEnhance(float sigmaS=10, float sigmaR=0.15f)
        {
            Cv2.DetailEnhance(_matSource, _matSource, sigmaS, sigmaR);
        }

        /// <summary>
        /// 铅笔素描
        /// </summary>
        /// <param name="pencilIndex">1,2 Default 1</param>
        /// <param name="sigmaS"></param>
        /// <param name="sigmaR"></param>
        /// <param name="shadeFactor"></param>
        public void PencilSketch(int pencilIndex=1,float sigmaS = 60, float sigmaR = 0.07f, float shadeFactor=0.02f)
        {
            var mat_a = new Mat();
            var mat_b = new Mat();
            Cv2.PencilSketch(_matSource, mat_a, mat_b, sigmaS, sigmaR, shadeFactor);
            if (pencilIndex == 1)
                _matSource = mat_a;
            else
                _matSource = mat_b;
        }

        /// <summary>
        /// 风格化
        /// </summary>
        /// <param name="sigmaS"></param>
        /// <param name="sigmaR"></param>
        public void Stylization(float sigmaS = 60f, float sigmaR = 0.45f)
        {
            Cv2.Stylization(_matSource, _matSource, sigmaS, sigmaR);
        }

        /// <summary>
        /// 调整颜色
        /// </summary>
        /// <param name="gainB"></param>
        /// <param name="gainG"></param>
        /// <param name="gainR"></param>
        public void Color(float gainB,float gainG,float gainR)
        {
            OpenCvSharp.XPhoto.CvXPhoto.ApplyChannelGains(_matSource, _matSource, gainB, gainG, gainR);
        }
        #endregion

        #region Dispose resource
        ~Image()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (!isDispose)
            {
                if (dispose)
                {
                    _matSource.Dispose();
                    _matSource = null;
                }
                isDispose = true;
            }
        }
        #endregion

        public enum ShowType
        {
            Original,
            Changed
        }
    }
}

