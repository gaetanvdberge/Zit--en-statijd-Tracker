using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace project2.View.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardContent : MasterDetailPage
    {
        public DashboardContent()
        {
            InitializeComponent();
        }
    }
}