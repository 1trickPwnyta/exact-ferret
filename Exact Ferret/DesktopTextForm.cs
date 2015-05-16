using Exact_Ferret.Settings_Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret
{
    public partial class DesktopTextForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
            int hWnd,               // handle to window
            int hWndInsertAfter,    // placement-order handle
            int X,                  // horizontal position
            int Y,                  // vertical position
            int cx,                 // width
            int cy,                 // height
            uint uFlags);           // window-positioning options

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpWindowClass, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        const int GWL_HWNDPARENT = -8;
        
        private void WinForm_Activated(object sender, System.EventArgs e)
        {
            SetWindowPos((int)Handle, 1, Left, Top, Width, Height, 0);

            IntPtr hprog = FindWindowEx(
                FindWindowEx(
                    FindWindow("Progman", "Program Manager"),
                    IntPtr.Zero, "SHELLDLL_DefView", ""
                ),
                IntPtr.Zero, "SysListView32", "FolderView"
            );

            SetWindowLong(this.Handle, GWL_HWNDPARENT, hprog);
        }

        private const int POINTER_CHECK_INTERVAL_MILLISECONDS = 1500;
        private const int FONT_SIZE = 16;
        private const int LINE_SIZE = 8;
        private const int PADDING = 16;
        private const int BOTTOM_PADDING = 32;

        private string pictureCaption = "";
        private string pictureDomain = "";
        private string pictureTerm = "";
        private string pointerFilePath;
        private DateTime lastPointerChange;

        public DesktopTextForm()
        {
            // If it's not enabled, quit
            if (!PropertiesManager.getEnableDesktopLabel())
                Environment.Exit(0);

            InitializeComponent();
            pointerFilePath = PropertiesManager.getPictureFolderPath() + "\\" + Desktop.CURRENT_PICTURE_POINTER_FILE_NAME;
            FileInfo pointerFile = new FileInfo(pointerFilePath);
            lastPointerChange = pointerFile.LastWriteTime;
            getImageData(Desktop.getCurrentPicturePath());

            new Thread(new ThreadStart(pointerFileCheckThread)).Start();
            Communication.startListening(1);
        }

        private void getImageData(string picturePath)
        {
            pictureCaption = PictureFileManager.getCaptionFromPicturePath(picturePath);
            pictureDomain = PictureFileManager.getDomainFromPicturePath(picturePath);
            pictureTerm = PictureFileManager.getSearchTermFromPicturePath(picturePath);
        }

        private void pointerFileCheckThread()
        {
            while (true)
            {
                Thread.Sleep(POINTER_CHECK_INTERVAL_MILLISECONDS);

                FileInfo pointerFile = new FileInfo(pointerFilePath);
                if (pointerFile.LastWriteTime > lastPointerChange)
                {
                    getImageData(Desktop.getCurrentPicturePath());
                    Invalidate(); // repaint
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = CreateGraphics();
            Pen pen = new Pen(Color.Black, 2);

            string text = "";
            int textHeight = 0;
            if (PropertiesManager.getShowCaption())
            {
                text += pictureCaption + "\r\n";
                textHeight += FONT_SIZE + LINE_SIZE;
            }
            if (PropertiesManager.getShowDomain())
            {
                text += pictureDomain + "\r\n";
                textHeight += FONT_SIZE + LINE_SIZE;
            }
            if (PropertiesManager.getShowTerm())
            {
                text += pictureTerm + "\r\n";
                textHeight += FONT_SIZE + LINE_SIZE;
            }

            int desktopLabelLocation = PropertiesManager.getDesktopLabelLocation();
            StringFormat stringFormat = new StringFormat();
            float x, y;

            switch (desktopLabelLocation)
            {
                case 0: // center
                    Location = new Point(Screen.GetBounds(this).Width/2 - Size.Width/2, Screen.GetBounds(this).Height/2 - Size.Height/2);
                    x = Size.Width / 2;
                    y = PADDING;
                    stringFormat.Alignment = StringAlignment.Center;
                    break;
                case 1: // top-right
                    Location = new Point(Screen.GetBounds(this).Width - Size.Width, 0);
                    x = Size.Width - PADDING;
                    y = PADDING;
                    stringFormat.Alignment = StringAlignment.Far;
                    break;
                case 2: // center-right
                    Location = new Point(Screen.GetBounds(this).Width - Size.Width, Screen.GetBounds(this).Height/2 - Size.Height/2);
                    x = Size.Width - PADDING;
                    y = PADDING;
                    stringFormat.Alignment = StringAlignment.Far;
                    break;
                case 3: // bottom-right
                    Location = new Point(Screen.GetBounds(this).Width - Size.Width, Screen.GetBounds(this).Height - Size.Height - BOTTOM_PADDING);
                    x = Size.Width - PADDING;
                    y = Size.Height - PADDING - textHeight;
                    stringFormat.Alignment = StringAlignment.Far;
                    break;
                case 4: // center-bottom
                    Location = new Point(Screen.GetBounds(this).Width/2 - Size.Width/2, Screen.GetBounds(this).Height - Size.Height - BOTTOM_PADDING);
                    x = Size.Width/2;
                    y = Size.Height - PADDING - textHeight;
                    stringFormat.Alignment = StringAlignment.Center;
                    break;
                case 5: // bottom-left
                    Location = new Point(0, Screen.GetBounds(this).Height - Size.Height - BOTTOM_PADDING);
                    x = PADDING;
                    y = Size.Height - PADDING - textHeight;
                    stringFormat.Alignment = StringAlignment.Near;
                    break;
                case 6: // center-left
                    Location = new Point(0, Screen.GetBounds(this).Height/2 - Size.Height/2);
                    x = PADDING;
                    y = PADDING;
                    stringFormat.Alignment = StringAlignment.Near;
                    break;
                case 7: // top-left
                    Location = new Point(0, 0);
                    x = PADDING;
                    y = PADDING;
                    stringFormat.Alignment = StringAlignment.Near;
                    break;
                case 8: // center-top
                    Location = new Point(Screen.GetBounds(this).Width / 2 - Size.Width / 2, 0);
                    x = Size.Width / 2;
                    y = PADDING;
                    stringFormat.Alignment = StringAlignment.Center;
                    break;
                default:
                    x = y = 0;
                    break;
            }

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(text, new Font("Arial", FONT_SIZE), new SolidBrush(Color.Black), x, y, stringFormat);
            g.DrawString(text, new Font("Arial", FONT_SIZE), new SolidBrush(Color.White), x + 2, y + 2, stringFormat);
            g.Dispose();
        }

        /*protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;
                return Params;
            }
        }*/

    }
}
