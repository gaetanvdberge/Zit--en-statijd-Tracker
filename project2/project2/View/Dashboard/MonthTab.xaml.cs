using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microcharts;
using Entry = Microcharts.Entry;
using SkiaSharp;
using project2.Model;

namespace project2.View.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MonthTab : ContentPage
    {

        public MonthTab()
        {
            InitializeComponent();

            #region Donut
            //lijst aanmaken met entries
            List<Entry> CirkelUren = new List<Entry>();

            //STATISCHE DATA
            CirkelUren.Add(new Entry(2000) { Color = SkiaSharp.SKColor.Parse("#cf6b00") });
            CirkelUren.Add(new Entry(400) { Color = SkiaSharp.SKColor.Parse("#F7F7F7") });

            //Donutchart aanmaken
            DonutChart createdDonut = new DonutChart { Entries = CirkelUren, MaxValue = 2400, LabelTextSize = 32, HoleRadius = 0.5F };
            #endregion

            #region Baar
            //lijst aanmaken met entries
            List<Entry> WeekDagStaven = new List<Entry>();

            //STATISCHE DATA
            WeekDagStaven.Add(new Entry(200) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Mon" });
            WeekDagStaven.Add(new Entry(100) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Tue" });
            WeekDagStaven.Add(new Entry(300) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Wed" });
            WeekDagStaven.Add(new Entry(150) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Thu" });
            WeekDagStaven.Add(new Entry(350) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Fri" });

            //barchart aanmaken
            BarChart createdBaar = new BarChart { Entries = WeekDagStaven, MaxValue = 400, LabelTextSize = 32};
            #endregion

            //de overviewlijst aanmaken
            List<Overview> overviewLijst = new List<Overview>();

            //Nieuw item aanmaken om in de lijst steken
            Overview newWeek = new Overview { Week = "22/01/2018 - 26/01/2018", Baar = createdBaar, Donut = createdDonut};

            //nieuwe itel in de lijst steken
            overviewLijst.Add(newWeek);

            //itemsource aan de listview geven
            lvwCharts.ItemsSource = overviewLijst;
        }

        private void lvwCharts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            lvwCharts.SelectedItem = null;
        }
    }
}