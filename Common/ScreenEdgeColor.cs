using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SyncLight.Common
{
    public class ScreenEdgeColor
    {
        public List<Color> GetScreenColor()
        {
            var colors = new List<Color>();

            // 获取主屏幕信息
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            var bounds = screen.Bounds;
            int screenWidth = bounds.Width;
            int screenHeight = bounds.Height;

            // 创建位图捕获屏幕
            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // 捕获当前屏幕内容
                    g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                }

                BitmapData data = bitmap.LockBits(
                    new Rectangle(0, 0, screenWidth, screenHeight),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                var totalColors = 35;
                var isThreeEdges = true;
                //消除黑边
                int EdgeOffset = 5;

                try
                {
                    if (isThreeEdges)
                    {
                        // 三边采样（上、左、右）
                        double topLength = screenWidth;
                        double leftLength = screenHeight;
                        double rightLength = screenHeight;
                        double totalLength = topLength + leftLength + rightLength;

                        int topCount = (int)Math.Round(totalColors * topLength / totalLength);
                        int leftCount = (int)Math.Round(totalColors * leftLength / totalLength);
                        int rightCount = totalColors - topCount - leftCount;

                        // 采样右边缘
                        for (int i = 0; i < rightCount; i++)
                        {
                            double ratio = rightCount > 1 ? (double)i / (rightCount - 1) : 0.5;
                            int y = (int)((screenHeight - EdgeOffset) * ratio);
                            Color rawColor = GetPixel(data, screenWidth - EdgeOffset, y);
                            colors.Add(AdjustSaturation(rawColor, SaturationFactor));
                        }

                        // 采样上边缘
                        for (int i = 0; i < topCount; i++)
                        {
                            double ratio = topCount > 1 ? (double)i / (topCount - 1) : 0.5;
                            int x = (int)((screenWidth - EdgeOffset) * ratio);
                            Color rawColor = GetPixel(data, x, 0);
                            colors.Add(AdjustSaturation(rawColor, SaturationFactor));
                        }

                        // 采样左边缘
                        for (int i = 0; i < leftCount; i++)
                        {
                            double ratio = leftCount > 1 ? (double)i / (leftCount - 1) : 0.5;
                            int y = (int)((screenHeight - EdgeOffset) * ratio);
                            Color rawColor = GetPixel(data, 0, y);
                            colors.Add(AdjustSaturation(rawColor, SaturationFactor));
                        }
                    }
                    else
                    {
                        // 四边采样（上、左、右、下）
                        double topLength = screenWidth;
                        double bottomLength = screenWidth;
                        double leftLength = screenHeight;
                        double rightLength = screenHeight;
                        double totalLength = topLength + bottomLength + leftLength + rightLength;

                        int topCount = (int)Math.Round(totalColors * topLength / totalLength);
                        int bottomCount = (int)Math.Round(totalColors * bottomLength / totalLength);
                        int leftCount = (int)Math.Round(totalColors * leftLength / totalLength);
                        int rightCount = totalColors - topCount - bottomCount - leftCount;

                        // 采样右边缘
                        for (int i = 0; i < rightCount; i++)
                        {
                            double ratio = rightCount > 1 ? (double)i / (rightCount - 1) : 0.5;
                            int y = (int)((screenHeight - EdgeOffset) * ratio);
                            Color rawColor = GetPixel(data, screenWidth - EdgeOffset, y);
                            colors.Add(AdjustSaturation(rawColor, SaturationFactor));
                        }

                        // 采样上边缘
                        for (int i = 0; i < topCount; i++)
                        {
                            double ratio = topCount > 1 ? (double)i / (topCount - 1) : 0.5;
                            int x = (int)((screenWidth - EdgeOffset) * ratio);
                            Color rawColor = GetPixel(data, x, 0);
                            colors.Add(AdjustSaturation(rawColor, SaturationFactor));
                        }

                        // 采样左边缘
                        for (int i = 0; i < leftCount; i++)
                        {
                            double ratio = leftCount > 1 ? (double)i / (leftCount - 1) : 0.5;
                            int y = (int)((screenHeight - EdgeOffset) * ratio);
                            Color rawColor = GetPixel(data, 0, y);
                            colors.Add(AdjustSaturation(rawColor, SaturationFactor));
                        }

                        // 采样下边缘
                        for (int i = 0; i < bottomCount; i++)
                        {
                            double ratio = bottomCount > 1 ? (double)i / (bottomCount - 1) : 0.5;
                            int x = (int)((screenWidth - EdgeOffset) * ratio);
                            Color rawColor = GetPixel(data, x, screenHeight - EdgeOffset);
                            colors.Add(AdjustSaturation(rawColor, SaturationFactor));
                        }
                    }
                }
                finally
                {
                    // 解锁位图数据
                    bitmap.UnlockBits(data);
                }

                return colors;
            }
        }

        private Color GetPixel(BitmapData data, int x, int y)
        {
            int stride = Math.Abs(data.Stride);
            IntPtr scan0 = data.Scan0;

            int byteOffset = y * stride + x * 4;
            byte[] pixelData = new byte[4];
            Marshal.Copy(scan0 + byteOffset, pixelData, 0, 4);

            return Color.FromArgb(
                pixelData[3],  // A
                pixelData[2],  // R
                pixelData[1],  // G
                pixelData[0]   // B
            );
        }


        // 饱和度增强系数 (1.0 = 原色, 1.5 = 增加50%, 2.0 = 翻倍)
        // 建议范围: 1.2f ~ 2.5f，过高会导致颜色失真或出现噪点
        private const float SaturationFactor = 2f;

        /// <summary>
        /// 调整颜色的饱和度
        /// </summary>
        /// <param name="color">原始颜色</param>
        /// <param name="factor">饱和度系数 (1.0=不变, >1.0=增加, <1.0=降低)</param>
        /// <returns>调整后的颜色</returns>
        private Color AdjustSaturation(Color color, float factor)
        {
            // 如果是灰度色或透明度为0，直接返回，避免计算错误或无意义
            if (color.R == color.G && color.G == color.B || color.A == 0)
            {
                return color;
            }

            // 1. RGB 转 HSL
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(Math.Max(r, g), b);
            float min = Math.Min(Math.Min(r, g), b);
            float l = (max + min) / 2f;
            float h, s;

            if (max == min)
            {
                h = 0;
                s = 0;
            }
            else
            {
                float d = max - min;
                s = l > 0.5f ? d / (2f - max - min) : d / (max + min);

                if (max == r)
                    h = (g - b) / d + (g < b ? 6f : 0f);
                else if (max == g)
                    h = (b - r) / d + 2f;
                else
                    h = (r - g) / d + 4f;

                h /= 6f;
            }

            // 2. 调整饱和度
            s *= factor;
            if (s > 1f) s = 1f; // 限制最大值为1，防止溢出

            // 3. HSL 转 RGB
            float rNew, gNew, bNew;

            if (s == 0)
            {
                rNew = gNew = bNew = l;
            }
            else
            {
                float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
                float p = 2f * l - q;

                rNew = HueToRgb(p, q, h + 1f / 3f);
                gNew = HueToRgb(p, q, h);
                bNew = HueToRgb(p, q, h - 1f / 3f);
            }

            return Color.FromArgb(
                color.A,
                (int)(rNew * 255f + 0.5f),
                (int)(gNew * 255f + 0.5f),
                (int)(bNew * 255f + 0.5f)
            );
        }

        private float HueToRgb(float p, float q, float t)
        {
            if (t < 0f) t += 1f;
            if (t > 1f) t -= 1f;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }
    }
}
