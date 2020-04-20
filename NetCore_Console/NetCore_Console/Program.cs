using System;
using System.Drawing;
using System.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace NetCore_Console
{
    class Program
    {
        const int FRAME_PER_SECONDS = 20;
        private static VideoCapture capture;
        static void Main(string[] args)
        {
            if (!CvInvoke.CheckLibraryLoaded())
                Console.WriteLine("CvInvoke.CheckLibraryLoaded() returned false!");
            capture = new VideoCapture(0);

            //whait 2 seconds for the cam to be activated
            
                dispatcherTimer = new Timer(tick, null, 2000, 1000 / FRAME_PER_SECONDS);
            Console.WriteLine("Programm is running.");
            Console.WriteLine("");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

       
        public delegate void OnFrameReceivedDelegate(object sender, byte[] jpegFrame);
        public static event OnFrameReceivedDelegate OnFrameReceived;
        private static byte[] getImage()
        {

            var captureImg = capture.QueryFrame().ToImage<Bgr, byte>();

            var grayImg = Hsv(captureImg);
            //blur(grayImg);
            //grayImg = threshold(grayImg);

            //  grayImg = converttoGray(captureImg);
            //grayImg = threshold(grayImg);
            //blur(grayImg);
            var image = grayImg.ToJpegData(50);
            return image;
        }

        private static void tick(object state)
        {
            try
            {
                var image = getImage();
                if (OnFrameReceived != null)
                    OnFrameReceived(new { }, image);
            }
            catch (Exception e)
            {

                Console.WriteLine("Error: " + e.ToString());
            }

        }

        private static Timer dispatcherTimer;
        private static Image<Gray, byte> converttoGray(Image<Bgr, byte> img)
        {
            return img.Convert<Gray, byte>();
        }
        private static void blur(Image<Gray, byte> img)
        {
            var sigma = 4;
            var size = 3;
            CvInvoke.GaussianBlur(img, img, new Size(size, size), sigma, sigma);
        }
        private static Image<Gray, byte> threshold(Image<Gray, byte> img)
        {
            return img.ThresholdAdaptive(new Gray(200), AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 21, new Gray(1));
        }
        private static Image<Gray, byte> Hsv(Image<Bgr, byte> img)
        {
            var hsvImg = img.Convert<Hsv, byte>();
            var hsvMask = hsvImg.InRange(new Hsv(160, 100, 100), new Hsv(179, 255, 255));
            return hsvMask;
        }
    }
}
