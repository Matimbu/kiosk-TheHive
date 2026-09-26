using System;
using System.Drawing;
using System.Windows.Forms;

namespace kiosk.UI
{
    public static class ProgressGuide
    {
        public static void Draw(Graphics g, Rectangle area, int step)
        {
            if (step < 1 || step > 3) return;
            string[] english = { "MENU", "ORDER", "PAY" };
            string[] filipino = { "MENU", "ORDER", "BAYAD" };
            int left = area.Left + 34;
            int right = area.Right - 34;
            int middle = (left + right) / 2;
            int[] marks = { left, middle, right };
            int lineY = area.Top + 7;

            using (Pen track = new Pen(Hive.Line, 2f))
                g.DrawLine(track, left, lineY, right, lineY);
            if (step > 1)
                using (Pen completed = new Pen(Hive.Teal, 2f))
                    g.DrawLine(completed, left, lineY, marks[step - 1], lineY);

            for (int i = 0; i < 3; i++)
            {
                bool current = i + 1 == step;
                bool completed = i + 1 < step;
                int radius = current ? 5 : 3;
                using (SolidBrush dot = new SolidBrush(current || completed ? Hive.Teal : Hive.Line))
                    g.FillEllipse(dot, marks[i] - radius, lineY - radius, radius * 2, radius * 2);

                string label = GuestText.Filipino ? filipino[i] : english[i];
                Hive.Text(g, label, Hive.Overline,
                    new Rectangle(marks[i] - 54, area.Top + 16, 108, 17),
                    current ? Hive.TealDarker : Hive.Muted, Hive.Centered);
            }
        }
    }
}
