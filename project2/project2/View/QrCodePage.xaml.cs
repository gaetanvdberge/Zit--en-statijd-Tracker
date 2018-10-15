using project2.Model;
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
	public partial class QrCodePage : ContentPage
	{
		public QrCodePage ()
		{
			InitializeComponent ();
            txtShareKey.Text = GlobalVariables.user.Sharekey;
            imgQrCode.Source = "https://api.qrserver.com/v1/create-qr-code/?data=" + GlobalVariables.user.Sharekey;

        }
	}
}