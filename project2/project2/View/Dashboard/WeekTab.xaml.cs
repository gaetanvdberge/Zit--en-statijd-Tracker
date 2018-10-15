using project2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microcharts;
using Entry = Microcharts.Entry;
using System.Diagnostics;

namespace project2.View.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeekTab : ContentPage
    {

        #region Aanmaken lijsten voor charts
        //01A - CODE VOOR DONUT (STA vs ZIT van de dag (480 minten --> (8*60)*5 -> 2400)
        List<Entry> staZitWeekEntry = new List<Entry>();

        //01B - CODE VOOR DE STAAFDIAGRAMMEN (in minuten dus max 400 min)
        List<Entry> WeekDagStaven = new List<Entry>();

        //02 - CODE VOOR DE CIRKEL (BV 8/40)
        List<Entry> CirkelUren = new List<Entry>();
        #endregion

        User requestedUser = new User();

        static DateTime datenow = DateTime.Now;
        static List<TimeStandingDay> list_maandag = new List<TimeStandingDay>();
        static List<TimeStandingDay> list_dinsdag = new List<TimeStandingDay>();
        static List<TimeStandingDay> list_woensdag = new List<TimeStandingDay>();
        static List<TimeStandingDay> list_donderdag = new List<TimeStandingDay>();
        static List<TimeStandingDay> list_vrijdag = new List<TimeStandingDay>();


        //Waarden staafdriagrammen --> 400 minuten in een dag
        int standMinutesMon;
        int standMinutesTue;
        int standMinutesWed;
        int standMinutesThu;
        int standMinutesFri;
        int weekTimePassed;

        static int vandaag = Convert.ToInt16(DateTime.Now.DayOfWeek);
        static int firstDay = ((datenow.Day) - vandaag) + 1;


        public WeekTab()
        {
            InitializeComponent();
            requestedUser = GlobalVariables.user;

            #region Colleague Code
            //Selected Colleague
            if (GlobalVariables.selectedColleague != null)
            {
                ColleagueFrame.IsVisible = true;
                imgColleagueProfile.Source = GlobalVariables.selectedColleague.UrlProfilePicture;
                lblColleagueName.Text = GlobalVariables.selectedColleague.Name;
                requestedUser = GlobalVariables.selectedColleague;

            }
            #endregion
            getData();

        }

        private async void getData()
        {
            DateTime datefirst = new DateTime(datenow.Year, datenow.Month, firstDay, 9, 0, 0);
            DateTime datelast = new DateTime(datenow.Year, datenow.Month, firstDay, 17, 0, 0);
            DateTime datumTotUur = new DateTime(datenow.Year, datenow.Month, firstDay, datenow.Hour, 0, 0);
            if (vandaag > 0) //maandag
            {
                list_maandag = await ImaniManager.GetTimeStandingDayAsync(requestedUser.UserId.ToString(), datefirst, datelast);
                for (int i = 0; i < list_maandag.Count(); i++) 
                {
                    if (list_maandag[i].Hour < datenow.Hour || (1 < vandaag)) 
                    {
                        standMinutesMon += Convert.ToInt16(list_maandag[i].TimeStandingSeconds);
                    }
                }
                standMinutesMon = standMinutesMon / 60; //om naar minuten te gaan, kan ook in 1lijn bij de if hierboven maar zo worden er minder bewerkingen gedaan
                if (vandaag != 1)//de verlopen tijd wordt bijgehouden om de cirkeldiagram te kunnen opmaken
                    weekTimePassed += 480;
                else
                    weekTimePassed = Convert.ToInt16((datumTotUur - datefirst).TotalMinutes);

            }
            if (vandaag > 1) //dinsdag
            {
                DateTime newFirst = datefirst.AddDays(1);
                DateTime newLast = datelast.AddDays(1);
                DateTime newdatumTotUur = datumTotUur.AddDays(1);
                list_dinsdag = await ImaniManager.GetTimeStandingDayAsync(requestedUser.UserId.ToString(), newFirst, newLast);
                for (int i = 0; i < list_dinsdag.Count() - 1; i++)
                {
                    if (list_dinsdag[i].Hour < datenow.Hour || (2 < vandaag))
                    {
                        standMinutesTue += Convert.ToInt16(list_dinsdag[i].TimeStandingSeconds);
                    }
                }
                standMinutesTue = standMinutesTue / 60; //om naar minuten te gaan
                if (vandaag != 2)
                    weekTimePassed += 480;
                else
                    weekTimePassed += Convert.ToInt16((newdatumTotUur - newFirst).TotalMinutes);
            }
            if (vandaag > 2) //woensdag
            {
                DateTime newFirst = datefirst.AddDays(2);
                DateTime newLast = datelast.AddDays(2);
                DateTime newdatumTotUur = datumTotUur.AddDays(2);

                list_woensdag = await ImaniManager.GetTimeStandingDayAsync(requestedUser.UserId.ToString(), newFirst, newLast);
                for (int i = 0; i < list_woensdag.Count() - 1; i++)
                {
                    if (list_woensdag[i].Hour < datenow.Hour || (3 < vandaag))
                    {
                        standMinutesWed += Convert.ToInt16(list_woensdag[i].TimeStandingSeconds);
                    }
                }
                standMinutesWed = standMinutesWed / 60; //om naar minuten te gaan
                if (vandaag != 3)
                    weekTimePassed += 480;
                else
                    weekTimePassed += Convert.ToInt16((newdatumTotUur - newFirst).TotalMinutes);
            }
            if (vandaag > 3) //donderdag
            {
                DateTime newFirst = datefirst.AddDays(3);
                DateTime newLast = datelast.AddDays(3);
                DateTime newdatumTotUur = datumTotUur.AddDays(3);
                list_donderdag = await ImaniManager.GetTimeStandingDayAsync(requestedUser.UserId.ToString(), newFirst, newLast);
                for (int i = 0; i < list_donderdag.Count() - 1; i++)
                {
                    if (list_donderdag[i].Hour < datenow.Hour || (4 < vandaag))
                    {
                    standMinutesThu += Convert.ToInt16(list_donderdag[i].TimeStandingSeconds);
                    }
                }
                standMinutesThu = standMinutesThu / 60; //om naar minuten te gaan
                if (vandaag != 4)
                    weekTimePassed += 480;
                else
                    weekTimePassed += Convert.ToInt16((newdatumTotUur - newFirst).TotalMinutes);
            }
            if (vandaag > 4) //vrijdag
            {
                DateTime newFirst = datefirst.AddDays(4);
                DateTime newLast = datelast.AddDays(4);
                DateTime newdatumTotUur = datumTotUur.AddDays(4);
                list_vrijdag = await ImaniManager.GetTimeStandingDayAsync(requestedUser.UserId.ToString(), newFirst, newLast);
                for (int i = 0; i < list_vrijdag.Count() - 1; i++)

                {
                    if (list_donderdag[i].Hour < datenow.Hour || (5 < vandaag))
                    {
                        standMinutesFri += Convert.ToInt16(list_vrijdag[i].TimeStandingSeconds);
                    }

                }
                standMinutesFri = standMinutesFri / 60; //om naar minuten te gaan
                if (vandaag != 5)
                    weekTimePassed += 480;
                else
                    
                    weekTimePassed += Convert.ToInt16((newdatumTotUur - newFirst).TotalMinutes);
            }


            #region DEZE WAARDEN MOETEN DUS AANGEPAST WORDEN
            //Donut chart --> 2400 minuts in a week
            int standMinutes = standMinutesMon + standMinutesTue + standMinutesWed + standMinutesThu + standMinutesFri;
            int sitMinutes = weekTimePassed - standMinutes;
            int StandHours=0;

            //Waarden voor cirkel met 40 uur
            //het aantal uren dat gestaan werd optellen
            for (int i = 0; i < list_maandag.Count - 1; i++)
            {
                if((list_maandag[i].TimeStandingSeconds != 0) && ((list_maandag[i].Hour < datenow.Hour) || (1 < vandaag)))
                {
                    StandHours += 1;
                }
            }
            for (int i = 0; i < list_dinsdag.Count - 1; i++)
            {
                if (list_dinsdag[i].TimeStandingSeconds != 0 && ((list_dinsdag[i].Hour < datenow.Hour) || (2 < vandaag)))
                {
                    StandHours += 1;

                }
            }
            for (int i = 0; i < list_woensdag.Count - 1; i++)
            {
                if (list_woensdag[i].TimeStandingSeconds != 0 && ((list_woensdag[i].Hour < datenow.Hour) || (3 < vandaag)))
                {
                    StandHours += 1;

                }
            }
            for (int i = 0; i < list_donderdag.Count - 1; i++)
            {
                if (list_donderdag[i].TimeStandingSeconds != 0 && ((list_donderdag[i].Hour < datenow.Hour) || (4 < vandaag)))
                {
                    StandHours += 1;

                }
            }
            for (int i = 0; i < list_vrijdag.Count - 1; i++)
            {
                if (list_vrijdag[i].TimeStandingSeconds != 0 && ((list_vrijdag[i].Hour < datenow.Hour) || (5 < vandaag)))
                {
                    StandHours += 1;

                }
            }

            //Waarden voor Tekst (40 uren in week)
            string tekstUrenZit = sitMinutes.ToString() ;
            //string teksUrenSta = standMinutes.ToString();
            #endregion

            #region Waarden aan charts toewijzen (No need to change)
            //Donut chart
            staZitWeekEntry.Add(new Entry(standMinutes) { Color = SkiaSharp.SKColor.Parse("#cf6b00") });
            staZitWeekEntry.Add(new Entry(sitMinutes) { Color = SkiaSharp.SKColor.Parse("#F9ECDF") });

            //Staafdriagrammen
            WeekDagStaven.Add(new Entry(standMinutesMon) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Mon" });
            WeekDagStaven.Add(new Entry(standMinutesTue) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Tue" });
            WeekDagStaven.Add(new Entry(standMinutesWed) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Wed" });
            WeekDagStaven.Add(new Entry(standMinutesThu) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Thu" });
            WeekDagStaven.Add(new Entry(standMinutesFri) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = "Fri" });


            //Waarden voor cirkel met 8 uur

            int CalculateRest = 40 - StandHours;
            CirkelUren.Add(new Entry(StandHours) { Color = SkiaSharp.SKColor.Parse("#cf6b00") });
            CirkelUren.Add(new Entry(CalculateRest) { Color = SkiaSharp.SKColor.Parse("#F7F7F7") });
            txtStandingHours.Text = StandHours.ToString(); //Deze waarde is voor de tekstbox in de ring
            int daysNotRecorded = 0;

            //Waarden voor Tekst (uren)
            // als er een hele dag geen recordings zijn wordt dit aangepast

            if (list_maandag.Count == 0 && list_dinsdag.Count == 0 && list_woensdag.Count == 0 && list_donderdag.Count == 0 && list_vrijdag.Count == 0)
            {

                lblUurStand.Text = "0h 0m";
                lblUurZit.Text = "0h 0m";
            }
            else
            {
                var som_standing_uur = standMinutes / 60;
                var som_standing_min = Convert.ToInt32(standMinutes % 60);
                var som_sitting_uur = (sitMinutes / 60) - (daysNotRecorded * 8);
                var som_sitting_min = sitMinutes % 60;
                lblUurStand.Text = (som_standing_uur + "h " + som_standing_min + "m").ToString(); //teksUrenSta;
                lblUurZit.Text = Convert.ToString(som_sitting_uur + "h " + som_sitting_min + "m"); //teksUren ;

            }
            if (list_maandag.Count == 0){ daysNotRecorded += 1;}
            if (list_dinsdag.Count == 0) { daysNotRecorded += 1; }
            if (list_woensdag.Count == 0) { daysNotRecorded += 1; }
            if (list_donderdag.Count == 0) { daysNotRecorded += 1; }
            if (list_vrijdag.Count == 0) { daysNotRecorded += 1; }
            
            


            //lblUurZit.Text = Convert.ToString(standing - som_standing_uur - stauur) + " h " + Convert.ToString(zitminuten - som_standing_min) + " m";

            #endregion

            #region Aanmaken Charts
            //----------------------------------------
            //Donut
            ChartsStaVsZit.Chart = new DonutChart { Entries = staZitWeekEntry, MaxValue = weekTimePassed, LabelTextSize = 32, HoleRadius = 0.5F };

            //Staven
            ChartStavenUren.Chart = new BarChart { Entries = WeekDagStaven, MaxValue = 400, LabelTextSize = 32 };

            // Cirkel van 8 uur
            ChartCirkelUren.Chart = new DonutChart { Entries = CirkelUren, MaxValue = 40, HoleRadius = 0.9F };
            //----------------------------------------
            #endregion

        }
    }
}