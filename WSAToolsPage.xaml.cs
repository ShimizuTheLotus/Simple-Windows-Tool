using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PInvoke;
using static SimpleWindowsTool.MainWindow;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWindowsTool
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WSAToolsPage : Page
    {
        public WSAToolsPage()
        {
            this.InitializeComponent();
        }

        async void WSAMusicGameModeChange()
        {
            if (AppRunningDatas.ComplexFunctions.WSAMusicGameMode)
            {
                ChangeDisplayOrientation(WinAPI.DMDO_DEFAULT);
                AppRunningDatas.WindowOrientation = WinAPI.DMDO_DEFAULT;
                //SetTaskbarState(AppBarStates.AutoHide);
                AppRunningDatas.showingTaskBar = true;

                SetTaskbarState(AppBarStates.AlwaysOnTop);
                await Task.Delay(300);
                SetTaskbarState(AppBarStates.AlwaysOnTop);
                AppRunningDatas.ComplexFunctions.WSAMusicGameMode = false;
            }
            else
            {
                ChangeDisplayOrientation(WinAPI.DMDO_180);
                AppRunningDatas.WindowOrientation = WinAPI.DMDO_180;
                SimpleWindowsToolGlobalSercives.HideTaskBar();
                AppRunningDatas.showingTaskBar = false;


                AppRunningDatas.ComplexFunctions.WSAMusicGameMode = true;
            }
        }

        public class SimpleWindowsToolGlobalSercives
        {
            public static async void HideTaskBar()
            {
                AppRunningDatas.showingTaskBar = false;
                while (!AppRunningDatas.showingTaskBar)
                {
                    await Task.Delay(200);
                    SetTaskbarState(AppBarStates.AutoHide);
                }
            }
        }




        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("shell32.dll")]
        public static extern UInt32 SHAppBarMessage(UInt32 dwMessage, ref APPBARDATA pData);

        public enum AppBarMessages
        {
            New = 0x00,
            Remove = 0x01,
            QueryPos = 0x02,
            SetPos = 0x03,
            GetState = 0x04,
            GetTaskBarPos = 0x05,
            Activate = 0x06,
            GetAutoHideBar = 0x07,
            SetAutoHideBar = 0x08,
            WindowPosChanged = 0x09,
            SetState = 0x0a
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize; // initialize this field using: Marshal.SizeOf(typeof(APPBARDATA));
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X
            {
                get { return Left; }
                set { Right -= (Left - value); Left = value; }
            }

            public int Y
            {
                get { return Top; }
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height
            {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width
            {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public System.Drawing.Point Location
            {
                get { return new System.Drawing.Point(Left, Top); }
                set { X = value.X; Y = value.Y; }
            }

            public System.Drawing.Size Size
            {
                get { return new System.Drawing.Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }

            public static implicit operator System.Drawing.Rectangle(RECT r)
            {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(System.Drawing.Rectangle r)
            {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj)
            {
                if (obj is RECT)
                    return Equals((RECT)obj);
                else if (obj is System.Drawing.Rectangle)
                    return Equals(new RECT((System.Drawing.Rectangle)obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((System.Drawing.Rectangle)this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }


        public enum AppBarStates
        {
            AlwaysOnTop = 0x00,
            AutoHide = 0x01
        }

        /// <summary>
        /// Set the Taskbar State option
        /// </summary>
        /// <param name="option">AppBarState to activate</param>
        public static void SetTaskbarState(AppBarStates option)
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = FindWindow("System_TrayWnd", null);
            msgData.lParam = (int)option;
            SHAppBarMessage((UInt32)AppBarMessages.SetState, ref msgData);
        }

        /// <summary>
        /// Gets the current Taskbar state
        /// </summary>
        /// <returns>current Taskbar state</returns>
        public AppBarStates GetTaskbarState()
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = FindWindow("System_TrayWnd", null);
            return (AppBarStates)SHAppBarMessage((UInt32)AppBarMessages.GetState, ref msgData);
        }


        //[DllImport("user32.dll")]
        //private static extern int FindWindow(string ClassName, string WindowName);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int handle, int cmdShow);

        private const int SW_HIDE = 0;//����
        private const int SW_SHOW = 5;//��ʾ

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, int iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);//��ȡ��Ļ��Ϣ
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, [In] ref DEVMODE lpDevMode, IntPtr hwnd, int dwFlags, IntPtr lParam); //���ú�����
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern long SetDisplayConfig(uint numPathArrayElements, IntPtr pathArray, uint numModeArrayElements, IntPtr modeArray, uint flags);  //�����������������Ļ�ĸ���ģʽ��������չģʽ��

        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;


            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            // public Point dmPosition;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;


            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;


            public short dmLogPixels;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        };

        public class WinAPI
        {
            // ƽ̨���õ�����
            [DllImport("user32.dll")]
            public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

            [DllImport("user32.dll")]
            public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

            // ���Ƹı���Ļ�ֱ��ʵĳ���
            public const int ENUM_CURRENT_SETTINGS = -1;
            public const int CDS_UPDATEREGISTRY = 0x01;
            public const int CDS_TEST = 0x02;

            public const int DISP_CHANGE_SUCCESSFUL = 0;
            public const int DISP_CHANGE_RESTART = 1;
            public const int DISP_CHANGE_FAILED = -1;

            // ���Ƹı䷽��ĳ�������
            public const int DMDO_DEFAULT = 0;
            public const int DMDO_90 = 1;
            public const int DMDO_180 = 2;
            public const int DMDO_270 = 3;
        }
        public static class ScreenSetting
        {
            /// <summary>
            /// �ı�ֱ���
            /// </summary>
            public static void ChangeResolution(int width, int height)
            {
                // ��ʼ�� DEVMODE�ṹ
                DEVMODE devmode = new DEVMODE();
                devmode.dmDeviceName = new String(new char[32]);
                devmode.dmFormName = new String(new char[32]);
                devmode.dmSize = (short)Marshal.SizeOf(devmode);

                if (0 != WinAPI.EnumDisplaySettings(null, WinAPI.ENUM_CURRENT_SETTINGS, ref devmode))
                {
                    devmode.dmPelsWidth = width;
                    devmode.dmPelsHeight = height;

                    // �ı���Ļ�ֱ���
                    int iRet = WinAPI.ChangeDisplaySettings(ref devmode, WinAPI.CDS_TEST);
                    //int iRet = NativeMethods.ChangeDisplaySettings(ref devmode, 0);
                    if (iRet == WinAPI.DISP_CHANGE_FAILED)
                    {
                        //����ʧ��
                    }
                    else
                    {
                        iRet = WinAPI.ChangeDisplaySettings(ref devmode, WinAPI.CDS_UPDATEREGISTRY);

                        switch (iRet)
                        {
                            // �ɹ��ı�
                            case WinAPI.DISP_CHANGE_SUCCESSFUL:
                                {
                                    break;
                                }
                            case WinAPI.DISP_CHANGE_RESTART:
                                {
                                    //MessageBox.Show("����Ҫ���������������ò�����Ч", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    break;
                                }
                            default:
                                {
                                    //MessageBox.Show("�ı���Ļ�ֱ���ʧ��", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    break;
                                }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �ı���ʾ��ʽ
        /// </summary>
        /// <param name="displayOrientation"></param>
        public static void ChangeDisplayOrientation(int displayOrientation)
        {
            // ��ʼ�� DEVMODE�ṹ
            DEVMODE devmode = new DEVMODE();
            devmode.dmDeviceName = new String(new char[32]);
            devmode.dmFormName = new String(new char[32]);
            devmode.dmSize = (short)Marshal.SizeOf(devmode);

            if (0 != WinAPI.EnumDisplaySettings(null, WinAPI.ENUM_CURRENT_SETTINGS, ref devmode))
            {
                int height = 0;
                int width = 0;
                switch (devmode.dmDisplayOrientation)
                {
                    case WinAPI.DMDO_DEFAULT:
                        height = devmode.dmPelsHeight;
                        width = devmode.dmPelsWidth;
                        break;
                    case WinAPI.DMDO_270:
                        width = devmode.dmPelsHeight;
                        height = devmode.dmPelsWidth;
                        break;
                    case WinAPI.DMDO_180:
                        height = devmode.dmPelsHeight;
                        width = devmode.dmPelsWidth;
                        break;
                    case WinAPI.DMDO_90:
                        width = devmode.dmPelsHeight;
                        height = devmode.dmPelsWidth;
                        break;
                    default:
                        // unknown orientation value
                        break;
                }

                int temp = devmode.dmPelsHeight;
                devmode.dmPelsHeight = devmode.dmPelsWidth;
                devmode.dmPelsWidth = temp;
                if (devmode.dmDisplayOrientation != displayOrientation)
                {
                    switch (displayOrientation)
                    {
                        case WinAPI.DMDO_DEFAULT:
                            devmode.dmPelsHeight = height;
                            devmode.dmPelsWidth = width;
                            devmode.dmDisplayOrientation = WinAPI.DMDO_DEFAULT;
                            break;
                        case WinAPI.DMDO_270:
                            devmode.dmPelsHeight = width;
                            devmode.dmPelsWidth = height;
                            devmode.dmDisplayOrientation = WinAPI.DMDO_270;
                            break;
                        case WinAPI.DMDO_180:
                            devmode.dmPelsHeight = height;
                            devmode.dmPelsWidth = width;
                            devmode.dmDisplayOrientation = WinAPI.DMDO_180;
                            break;
                        case WinAPI.DMDO_90:
                            devmode.dmPelsHeight = width;
                            devmode.dmPelsWidth = height;
                            devmode.dmDisplayOrientation = WinAPI.DMDO_90;
                            break;
                        default:
                            // unknown orientation value
                            break;
                    }
                }
                else
                {
                    return;
                }

                // �ı���Ļ��ʾ��ʽ
                int iRet = WinAPI.ChangeDisplaySettings(ref devmode, 0);
                if (iRet == WinAPI.DISP_CHANGE_FAILED)
                {
                    //MessageBox.Show("����ִ���������", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    iRet = WinAPI.ChangeDisplaySettings(ref devmode, WinAPI.CDS_UPDATEREGISTRY);

                    switch (iRet)
                    {
                        // �ɹ��ı�
                        case WinAPI.DISP_CHANGE_SUCCESSFUL:
                            {
                                break;
                            }
                        case WinAPI.DISP_CHANGE_RESTART:
                            {
                                //MessageBox.Show("����Ҫ���������������ò�����Ч", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        default:
                            {
                                //MessageBox.Show("�ı���Ļ�ֱ���ʧ��", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }

                    }
                }
            }

        }

        private void WSAMusicGameModeChangeStatusButton_Toggled(object sender, RoutedEventArgs e)
        {
            WSAMusicGameModeChange();
        }

        private void OpenWSAAndroidSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = "start wsa://com.android.settings";

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //�Ƿ�ʹ�ò���ϵͳshell����
                p.StartInfo.RedirectStandardInput = true;//�������Ե��ó����������Ϣ
                p.StartInfo.RedirectStandardOutput = true;//�ɵ��ó����ȡ�����Ϣ
                p.StartInfo.RedirectStandardError = true;//�ض����׼�������
                p.StartInfo.CreateNoWindow = true;//����ʾ���򴰿�
                p.Start();//��������

                //��cmd���ڷ���������Ϣ
                p.StandardInput.WriteLine(str + "&exit");

                p.StandardInput.AutoFlush = true;
                p.StandardInput.WriteLine("exit");
                //���׼����д��Ҫִ�е��������ʹ��&������������ķ��ţ���ʾǰ��һ��������Ƿ�ִ�гɹ���ִ�к���(exit)��������ִ��exit����������ReadToEnd()���������
                //ͬ��ķ��Ż���&&��||ǰ�߱�ʾ����ǰһ������ִ�гɹ��Ż�ִ�к����������߱�ʾ����ǰһ������ִ��ʧ�ܲŻ�ִ�к��������



                //��ȡcmd���ڵ������Ϣ
                string output = p.StandardOutput.ReadToEnd();

                //StreamReader reader = p.StandardOutput;
                //string line=reader.ReadLine();
                //while (!reader.EndOfStream)
                //{
                //    str += line + "  ";
                //    line = reader.ReadLine();
                //}

                p.WaitForExit();//�ȴ�����ִ�����˳�����
                p.Close();
            }
            catch { }
        }
    }
}
