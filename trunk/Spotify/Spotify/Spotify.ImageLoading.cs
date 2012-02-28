using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotiFire.SpotifyLib;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using centrafuse.Plugins;   

namespace Spotify
{
    public partial class Spotify
    {
        private string currentImageId;
        private Image currentImage;
        private void LoadImage(string imageId)
        {
            currentImageId = imageId;
            
            CF_clearPictureImage("pictureBox");
            if (currentImage != null)
                currentImage.Dispose();

            if (!string.IsNullOrEmpty(imageId))
            {
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    try
                    {
                        var image = SpotifySession.GetImageFromId(imageId);
                        image.WaitForLoaded();
                        if (imageId.Equals(currentImageId))
                        {
                            var imageObject = image.GetImage();
                            if (imageId.Equals(currentImageId))
                            {
                                imageObject = ResizeToFitBox(imageObject);
                                this.BeginInvoke(new MethodInvoker(delegate()
                                {
                                    if (imageId.Equals(currentImageId))
                                    {
                                        currentImage = imageObject;
                                        CF_setPictureImage("pictureBox", imageObject);
                                    }
                                    else
                                    {
                                        imageObject.Dispose();
                                    }
                                }));
                            }
                        }
                        
                    }
                    catch { return; }
                });
            }
        }

        private const int pictureBoxSize = 113;
        private Image ResizeToFitBox(Image imageObject)
        {
            Rectangle drawRectangle;
            if (imageObject.Width > imageObject.Height)
            {
                double factor = (double)pictureBoxSize / imageObject.Width;
                int resizeHeight = (int)Math.Ceiling(imageObject.Height * factor);
                drawRectangle = new Rectangle(0, (pictureBoxSize - resizeHeight) / 2, pictureBoxSize, resizeHeight);
            }
            else
            {
                double factor = (double)pictureBoxSize / imageObject.Height;
                int resizeWidth = (int)Math.Ceiling(imageObject.Width * factor);
                drawRectangle = new Rectangle((pictureBoxSize - resizeWidth) / 2, 0, resizeWidth, pictureBoxSize);
            }

            Bitmap resized = new Bitmap(pictureBoxSize, pictureBoxSize);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.Clear(Color.Black);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                g.DrawImage(imageObject, drawRectangle);
                g.Flush();
            }
            imageObject.Dispose();
            return resized;
        }
    }
}
