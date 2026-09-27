using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace kiosk.UI
{
    // A consistent 24-unit line icon set, drawn at native resolution.
    internal static class CategoryIcons
    {
        public static void Draw(Graphics g, string category, RectangleF bounds, Color color)
        {
            var state = g.Save();
            g.TranslateTransform(bounds.X, bounds.Y);
            g.ScaleTransform(bounds.Width / 24f, bounds.Height / 24f);
            using (var p = new Pen(color, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
            {
                switch (category)
                {
                    case "Best Sellers":
                        var star = new PointF[10];
                        for (int i = 0; i < star.Length; i++) {
                            double angle = -Math.PI / 2 + i * Math.PI / 5;
                            float radius = i % 2 == 0 ? 10 : 4.6f;
                            star[i] = new PointF(12 + radius * (float)Math.Cos(angle), 12 + radius * (float)Math.Sin(angle));
                        }
                        g.DrawPolygon(p, star);
                        break;
                    case "Coffee":
                        g.DrawLines(p, new[] { new PointF(3, 9), new PointF(4, 18), new PointF(15, 18), new PointF(16, 9), new PointF(3, 9) });
                        g.DrawArc(p, 14, 10, 7, 6, -90, 180);
                        g.DrawLine(p, 2, 21, 19, 21);
                        g.DrawBezier(p, 7, 6, 4, 3, 10, 3, 7, 1);
                        g.DrawBezier(p, 12, 6, 9, 3, 15, 3, 12, 1);
                        break;
                    case "Non-Coffee":
                        g.DrawLines(p, new[] { new PointF(5, 7), new PointF(7, 22), new PointF(17, 22), new PointF(19, 7), new PointF(5, 7) });
                        g.DrawLines(p, new[] { new PointF(12, 13), new PointF(14, 2), new PointF(20, 2) });
                        g.DrawLine(p, 7, 13, 17, 13);
                        break;
                    case "Classics":
                        g.DrawLines(p, new[] { new PointF(5, 6), new PointF(7, 22), new PointF(17, 22), new PointF(19, 6), new PointF(5, 6) });
                        g.DrawLine(p, 4, 4, 20, 4); g.DrawLine(p, 13, 1, 12, 12);
                        g.DrawEllipse(p, 8, 16, 2, 2); g.DrawEllipse(p, 13, 17, 2, 2); g.DrawEllipse(p, 11, 13, 2, 2);
                        break;
                    case "Cheesecake":
                        // a malt drink, not a cake: the same cup as the other drinks,
                        // with the domed cheesecake cream above the rim and its swirl inside
                        g.DrawLines(p, new[] { new PointF(5, 8), new PointF(7, 22), new PointF(17, 22), new PointF(19, 8), new PointF(5, 8) });
                        g.DrawArc(p, 7, 3, 10, 10, 180, 180);   // heaped cream, narrower than the rim - a topping, not a lid
                        g.DrawEllipse(p, 9.6f, 5.3f, 1.3f, 1.3f); g.DrawEllipse(p, 13.1f, 5.7f, 1.3f, 1.3f);   // cocoa on top, so it reads as food, not a handle
                        g.DrawBezier(p, 6.3f, 15, 9.5f, 12.5f, 13.5f, 17.5f, 17.7f, 14.5f);
                        break;
                    case "GentleTea":
                        g.DrawBezier(p, 4, 20, 0, 6, 13, 3, 21, 3);
                        g.DrawBezier(p, 21, 3, 21, 17, 13, 23, 4, 20);
                        g.DrawLine(p, 3, 22, 17, 7); g.DrawLine(p, 9, 15, 9, 9); g.DrawLine(p, 9, 15, 15, 15);
                        break;
                    case "Rice Meals":
                        g.DrawArc(p, 3, 7, 18, 14, 0, 180); g.DrawLine(p, 3, 14, 21, 14);
                        g.DrawArc(p, 5, 6, 14, 12, 180, 180); g.DrawLine(p, 9, 22, 15, 22);
                        g.DrawLine(p, 9, 9, 10, 10); g.DrawLine(p, 14, 9, 15, 10);
                        break;
                }
            }
            g.Restore(state);
        }
    }
}
