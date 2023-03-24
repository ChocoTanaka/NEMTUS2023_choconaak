using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using ZXing;

namespace SymbolAuthenticater
{
    public partial class SymbolAuthenticater : Form
    {
        Mat frame;
        VideoCapture capture;
        string text,prevData;
        readonly string node = "https://mikun-testnet.tk:3001";

        public SymbolAuthenticater()
        {
            InitializeComponent();
        }

        private async void SymbolAuthenticater_Load(object sender, EventArgs e)
        {

            for (int i = 0, max = 10; i< max; i++)
            {
                //カメラ画像取得用のVideoCapture作成
                capture = new VideoCapture(i); // 0がインカメ, 1以降がウェブカメラ
                if (capture.IsOpened())
                {
                    break;
                }
            }
            //画像取得用のMatを作成
            frame = new Mat();
            while (true)
            {
                try
                {
                    capture.Read(frame);
                    if (frame.Empty())
                    {
                        break;
                    }

                    if (frame.Size().Width > 0)
                    {
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        //PictureBoxに表示　MatをBitMapに変換
                        Bitmap bmp = BitmapConverter.ToBitmap(frame);
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        pictureBox.Image = bmp;
                        // QRコードの解析
                        BarcodeReader reader = new BarcodeReader();
                        Result result = reader.Decode(bmp);
                        if (result != null && prevData != result.Text)
                        {
                            text = result.Text;
                            prevData = text;

                            string seatresult = Symbol.GetDataFromAPI(node, text);
                            if(seatresult == "E1")
                            {
                                Answer_Seat.Text = $"QRコードが違います。";
                            }
                            else if(seatresult == null)
                            {
                                Answer_Seat.Text = $"チケットを持ってません。";
                                Console.WriteLine("ないよ");
                            }
                            else
                            {
                                Answer_Seat.Text = $"あなたは{seatresult}です。";
                            }
                            
                        }
                    }
                        await Task.Delay(150);
                    if (this.IsDisposed)
                    {
                        break;
                    }
                }

                catch (Exception)
                {
                    break;
                }
            }

        }
    }
}
