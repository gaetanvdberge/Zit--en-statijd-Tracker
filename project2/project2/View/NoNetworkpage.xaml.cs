using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace project2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoNetworkpage : ContentPage
    {
        public NoNetworkpage()
        {
            InitializeComponent();
            BackgroundColor = Color.FromRgb(0xf0, 0xf0, 0xf0);
            Content = new Label()
            {
                Text = "No Network Connection Available",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.FromRgb(0x40, 0x40, 0x40)
            };
        }
    }
}