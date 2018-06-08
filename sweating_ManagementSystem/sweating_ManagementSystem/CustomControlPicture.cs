using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sweating_ManagementSystem
{
    class CCP
    {
        //==============================
        //
        //dirPath:画像保存先ディレクトリ
        //exePath:exe保存先ディレクトリ
        //
        //==============================
        /// <summary>
        /// 画像保存先ディレクトリ
        /// </summary>
        public String dirPath;
        /// <summary>
        /// 実行ファイル保存先ディレクトリ
        /// </summary>
        public String exePath;

        //==============================
        //
        //コンストラクタ
        //exeファイル保存ディレクトリ下の
        //フォルダを指定
        //
        //==============================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fName">画像保存先フォルダ名(無指定の場合は"none")</param>
        public CCP(String fName)
        {
            PathInit(fName);
        }

        //==============================
        //
        //パス取得(初期化)
        //
        //==============================
        /// <summary>
        /// パス取得
        /// </summary>
        /// <param name="folderName"></param>
        protected void PathInit(String folderName)
        {
            dirPath = String.Format(System.Reflection.Assembly.GetExecutingAssembly().Location);
            dirPath = System.IO.Path.GetDirectoryName(dirPath);
            exePath = dirPath;
            if (folderName != "none")
            {
                dirPath += "\\" + folderName + "\\";
            }
        }

        //==============================
        //
        //ARGBの変更(画像再取得)
        //
        //==============================
        /// <summary>
        /// ARGBの変更
        /// </summary>
        /// <param name="img">画像</param>
        /// <param name="picName">画像の名前</param>
        /// <param name="R">赤</param>
        /// <param name="G">緑</param>
        /// <param name="B">青</param>
        /// <param name="A">アルファ</param>
        /// <returns></returns>
        public Image ImageChange(Image img, String picName, float R, float G, float B, float A)
        {
            Image image = Image.FromFile(String.Format(dirPath + picName));
            Bitmap _img = new Bitmap(img.Width, img.Height);
            Graphics g = Graphics.FromImage(_img);

            ColorMatrix cm = new ColorMatrix();

            //******************************
            //00,11,22,33,44それぞれ,RGBAW
            //適宜変更(0.0f～1.0f)
            //******************************
            cm.Matrix00 = R;
            cm.Matrix11 = G;
            cm.Matrix22 = B;
            cm.Matrix33 = A;
            cm.Matrix44 = 1;

            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);

            image.Dispose();
            g.Dispose();

            return _img;
        }
    }
}
