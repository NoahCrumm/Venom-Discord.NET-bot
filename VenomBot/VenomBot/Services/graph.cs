using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace VenomBot.Services
{
    public class Graph
    {
        //Default Values
        const string www = "https://image-charts.com/chart?";
        public string cht = "bhg"; //Chart Type
        public string chd = "t%3A60%2C40%2C30%7C28%2C99%2C30"; //Data
        public string chds = "a";
        public string chxr = "1%2C20%2C100"; //Axis Range
        public string chxp = "";
        public string chof = ".png";
        public string chs = "300x500";
        public string chdl = "Level%7CInfluence";
        public string chdls = ""; //chart legend text and style (non functional on bar charts?)
        public string chg = "";
        public string chco = ""; //Series Colors
        public string chtt = "All Stats"; //Title
        public string chts = "000000%2C20"; //chart <color, size>
        public string chxt = "x%2Cy"; //visible axes
        public string chxl = "0%3A%7Ctest%7C%7Ctest2%7C%7Ctest3%7Ctest4%7Ctest5%7Ctest6"; //axis labels
        public string chxs = "";
        public string chm = "B,76A4FB,0,0,0";
        public string chls = "";
        public string chl = "";
        public string chma = "";
        public string chdlp = "";
        public string chf = "b0%2Clg%2C90%2C03a9f4%2C0%2C3f51b5%2C1%7Cbg%2Cs%2Cefefef%7Cb1%2Clg%2C90%2C03f45f%2C0%2C3fb560%2C1"; //<fill_type>,lg,<angle>,<color_1>,<color_centerpoint_1>
        public string chbh = ""; //Bar Width Spacing (not supported)
        public string icwt = ""; //watermark.  (doesnt mater.  removed with paid subscr)

        public string ToUrl()
        {
            var query = "";
            foreach (FieldInfo field in GetType().GetFields())
            {
                var n = field.Name;
                var v = field.GetValue(this) as string;

                if (!string.IsNullOrEmpty(v))
                    query += $"{n}={v}&";
            }
            return www + query;
        }

        public static async Task CombineAsync(List<string> urls, ISocketMessageChannel channel, string message = "")
        {
            List<Bitmap> images = new List<Bitmap>();
            Bitmap finalImage = null;
            try
            {
                int width = 0;
                int height = 0;
                foreach (var url in urls)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if ((response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.Redirect) &&
                    response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                    {
                        var bitmap = new Bitmap(response.GetResponseStream());

                        bitmap = (height == 0) ? bitmap : bitmap.Crop(new Rectangle(0, 50, bitmap.Width, bitmap.Height));
                        //update the size of the final bitmap
                        width = bitmap.Width > width ? bitmap.Width : width;
                        height += bitmap.Height;

                        images.Add(bitmap);
                    }
                    else
                        Console.WriteLine("Failed Response");
                }

                finalImage = new System.Drawing.Bitmap(width, height);

                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image,
                          new System.Drawing.Rectangle(0, offset, image.Width, image.Height));
                        offset += (image.Height);
                    }
                }

                finalImage.Save("testImage.png", ImageFormat.Png);

                await channel.SendFileAsync("testImage.png", message);
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                    finalImage.Dispose();

                Console.WriteLine(ex.Message);
            }
            finally
            {
                images.ForEach(r => r.Dispose());
            }
        }
    }

    public static class GraphExtentions
    {
        public static Graph SetTitle(this Graph graph, string t)
        {
            graph.chtt = t;
            return graph;
        }
        public static Graph SetType(this Graph graph, string t)
        {
            graph.cht = t;
            return graph;
        }
        public static Graph SetLineFills(this Graph graph, string t)
        {
            graph.chm = t;
            return graph;
        }

        public static Graph SetBottomLabels(this Graph graph, List<string> labels)
        {
            graph.chdl = string.Join("|", labels);
            return graph;
        }
        public static Graph SetAxisLabels(this Graph graph, int axisIndex, List<string> labels)
        {
            var l = string.Join("|", labels);
            graph.chxl = $"{axisIndex}:|{l}";
            return graph;
        }

        public static Graph SetTitleColorSize(this Graph graph, string color, int size)
        {
            //TODO suport multiple axis divided by |
            graph.chts = $"{color},{size}";
            return graph;
        }

        public static Graph SetAxisRange(this Graph graph, int axisIndex, int min, int max)
        {
            //TODO suport multiple axis divided by |
            graph.chxr = $"{axisIndex},{min},{max}";
            return graph;
        }

        public static Graph SetData(this Graph graph, List<int> data1, List<int> data2)
        {
            var d1 = string.Join(",", data1);
            var d2 = string.Join(",", data2);
            graph.chd = $"t:{d1}|{d2}";
            return graph;
        }
        public static Graph SetData(this Graph graph, List<int> data1)
        {
            var d1 = string.Join(",", data1);
            graph.chd = $"t:{d1}";
            return graph;
        }

        public static Graph SetChartSize(this Graph graph, int x, int y)
        {
            y = (y > 999) ? 999 : y;
            x = (x > 999) ? 999 : x;
            graph.chs = $"{x}x{y}";
            return graph;
        }
        public static Graph SetMargin(this Graph graph, int l, int r, int t = 0, int b = 0)
        {
            graph.chma = $"{l},{r},{t},{b}";
            return graph;
        }

        public static async Task SendChartAsync(this Graph graph, ISocketMessageChannel channel, string url, string message = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();


                // Check that the remote file was found. The ContentType
                // check is performed since a request for a non-existent
                // image file might be redirected to a 404-page, which would
                // yield the StatusCode "OK", even though the image was not
                // found.
                if ((response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.Redirect) &&
                    response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {

                    // if the remote file was found, download it
                    using (Stream inputStream = response.GetResponseStream())
                    {
                        await channel.SendFileAsync(inputStream, "stats.png", message);
                    }
                }
                else
                    Console.WriteLine("Failed Response");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "          " + url);
            }
        }
        public static Bitmap Crop(this Image originalImage, Rectangle cropBounds)
        {
            Bitmap croppedImage =
                new Bitmap(cropBounds.Width - cropBounds.X, cropBounds.Height - cropBounds.Y);

            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(originalImage,
                    0, 0,
                    cropBounds,
                    GraphicsUnit.Pixel);
            }

            return croppedImage;
        }
    }

    public static class GraphType
    {
        public const string Line = "ls";
        public const string HorizontalBar = "bhg";
    }
}
