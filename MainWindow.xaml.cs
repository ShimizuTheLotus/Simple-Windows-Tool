using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Display.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using WinRT.Interop;
using static SimpleWindowsTool.WSAToolsPage;
using static System.Net.WebRequestMethods;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWindowsTool
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //Variables
        private AppWindow m_AppWindow;

        public class AppRunningDatas
        {
            public static bool showingTaskBar = true;
            public static int WindowOrientation = WinAPI.DMDO_DEFAULT;
            public class ComplexFunctions
            {
                public static bool WSAMusicGameMode = false;
            }
        }

        public MainWindow()
        {
            this.InitializeComponent();


            //Initialize
            ChangeDisplayOrientation(WinAPI.DMDO_DEFAULT);
            SetTaskbarState(AppBarStates.AlwaysOnTop);
            MainPageNavigationView.SelectedItem = NavigationViewItem_LiveTiles;
            ContentFrame.Navigate(typeof(LiveTilesPage));
            //ChangeLiveTilesLogoBackground();
            ExtendIntoTitleBar();
        }


        /// <summary>
        /// Extend panel into titlebar in Windows APP SDK
        /// </summary>
        //https://learn.microsoft.com/en-us/windows/apps/develop/title-bar#how-much-to-customize-the-title-bar
        public void ExtendIntoTitleBar()
        {
            //extendscontentintotitlebar = true;
            //settitlebar(apptitlebar);
            m_AppWindow = GetAppWindowForCurrentWindow();
            var titleBar = m_AppWindow.TitleBar;
            // Hide system title bar.
            titleBar.ExtendsContentIntoTitleBar = true;
            titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
            //titleBar.BackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        private void ChangeLiveTilesLogoBackground()
        {
            // 加载透明背景的图像

            Windows.ApplicationModel.Resources.Core.ResourceContext.SetGlobalQualifierValue("Contrast", "high");
            var namedResource = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap[@"Files/Assets/LiveTilesLogo.png"];
            //this.myXAMLImageElement.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(namedResource.Uri);

            Bitmap image = new Bitmap(namedResource.Uri.ToString());
            if(image != null)
            {
                int i = 0;
                i = 0 / i;
            }

            // 创建一个与图像大小相同的空白位图
            Bitmap result = new Bitmap(image.Width, image.Height);

            // 使用Graphics对象将图像绘制到空白位图上
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.Clear(System.Drawing.Color.Red);  // 这里将背景颜色设置为红色，你可以替换为其他颜色
                graphics.DrawImage(image, 0, 0, image.Width, image.Height);
            }

            // 保存结果图像
            result.Save("ms-appx:///Assets/LiveTilesLogoAdapted,png");


            // 释放资源
            image.Dispose();
            result.Dispose();
        }

        private void MainPageNavigationView_Navigate(Type navPageType,NavigationTransitionInfo transitionInfo)
        {
            Type preNavPageType = ContentFrame.CurrentSourcePageType;
            if(navPageType is not null && !Type.Equals(preNavPageType, navPageType))
            {
                ContentFrame.Navigate(navPageType, null, transitionInfo);
            }
        }

        private void MainPageNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            //Type pageType;
            if(args.IsSettingsInvoked == true)
            { }
            else if(args.InvokedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                MainPageNavigationView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
            }
            //ContentFrame.NavigateToType(pageType, null, navOptions);
        }
    }
}
