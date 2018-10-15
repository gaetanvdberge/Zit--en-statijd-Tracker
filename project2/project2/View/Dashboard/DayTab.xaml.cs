using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microcharts;
using Entry = Microcharts.Entry;
using project2.Model;

namespace project2.View.Dashboard
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DayTab : ContentPage
    {
        #region Aanmaken lijsten voor charts
        //01A - CODE VOOR DONUT (STA vs ZIT van de dag (480 minten --> (8x60))
        List<Entry> staZitDagEntry = new List<Entry>();

        //01B - CODE VOOR DE STAAFDIAGRAMMEN (in minuten dus max 60)
        List<Entry> dagUren = new List<Entry>();

        //02 - CODE VOOR DE CIRKEL (BV 3/8)
        List<Entry> urenEntry = new List<Entry>();
        #endregion
        User requestedUser = new User();

        public DayTab()
        {
            InitializeComponent();

            //nieuwe gebruiker aanmaken en standaard gelijk stellen aan globalUser
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

            //Waarden ophalen via Azure Functie & dashboard aanvullen
            GetTimestandingDayAsync();
        }

        private async void GetTimestandingDayAsync()
        {
            #region Object lijst opvullen
            DateTime datenow = DateTime.Now;
            List<TimeStandingDay> list_timestandingday = new List<TimeStandingDay>();
            //Datum van vandaag klaarmaken om te versturen
            int date_last_hour = 0;
            int zitminuten = 60;
            int stauur = 1;



            DateTime datefirst = new DateTime(datenow.Year, datenow.Month, datenow.Day, 9, 0, 0);

            //Lijst ophalen van vandaag per uur
            if (datenow.Hour >= 9 && datenow.Hour < 17) // enkel de data tot en met het vorige uur worden weergegeven --> deze worden in een lijst gestoken
            {
                date_last_hour = datenow.Hour;
                DateTime datelast = new DateTime(datenow.Year, datenow.Month, datenow.Day, 17, 0, 0);
                list_timestandingday = await ImaniManager.GetTimeStandingDayAsync(requestedUser.UserId.ToString(), datefirst, datelast);
                int teVerwijderenIndex = date_last_hour - 9;

                if (list_timestandingday.Count == 0)
                {
                    zitminuten = 0;
                    stauur = 0;

                }
                else
                {
                    for (int i = date_last_hour; i < 17; i++)
                    {

                        list_timestandingday.RemoveAt(teVerwijderenIndex);       
                        TimeStandingDay emptyTime = new TimeStandingDay { Hour = i, TimeStandingSeconds = 0 };
                        list_timestandingday.Add(emptyTime);
                    }
                }
            }
           
            else if(datenow.Hour >= 17) // hele dag wordt uitgevoerd
            {
                date_last_hour = 17;
                DateTime datelast = new DateTime(datenow.Year, datenow.Month, datenow.Day, date_last_hour, 0, 0);
                list_timestandingday = await ImaniManager.GetTimeStandingDayAsync(requestedUser.UserId.ToString(), datefirst, datelast);
                if (list_timestandingday.Count == 0)
                {
                    list_timestandingday.Clear();
                    for (int i = 9; i < 17; i++)
                    {
                        TimeStandingDay emptyTime = new TimeStandingDay { Hour = i, TimeStandingSeconds = 0 };
                        list_timestandingday.Add(emptyTime);
                    }
                    zitminuten = 0;
                    stauur = 0;
                }
            }
            else if (datenow.Hour < 9) //alles van vandaag zal nog op 0 staan, leeg zijn.
            {
                for (int i = 9; i < 17; i++)
                {
                    TimeStandingDay emptyTime = new TimeStandingDay { Hour = i, TimeStandingSeconds = 0 };
                    list_timestandingday.Add(emptyTime);
                }
                zitminuten = 0;
                stauur = 0;
            }
            

            #endregion

            #region Grafieken opuvllen met waarden
            //Opvullen Staaf diagrammen ----------------------------

            for (int x = 0; x <= list_timestandingday.Count - 1; x++) //de waarden klaarstomen om per staaf weer te geven
            {
                if (list_timestandingday[x].TimeStandingSeconds == 0 && list_timestandingday[x].Hour < datenow.Hour)
                {
                    dagUren.Add(new Entry(1) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = list_timestandingday[x].Hour.ToString() });

                }
                else
                {
                dagUren.Add(new Entry(Convert.ToInt64(list_timestandingday[x].TimeStandingSeconds / 60)) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = list_timestandingday[x].Hour.ToString() });
                }
            }
            if (list_timestandingday.Count == 0)
            {
                for (int x = 9; x < 17; x++)
                {
                    dagUren.Add(new Entry(0) { Color = SkiaSharp.SKColor.Parse("#cf6b00"), Label = x.ToString() });
                }
            }
            ChartStavenUren.Chart = new BarChart { Entries = dagUren, MaxValue = 60, LabelTextSize = 32 };

            //Opvullen cirkeldiagram -------------------------------
            Double som_standing = 0;
            int totaalUur = 3600;
            for (int x = 0; x <= list_timestandingday.Count - 1; x++)
            {
                som_standing += list_timestandingday[x].TimeStandingSeconds;
               
                if (list_timestandingday[x].TimeStandingSeconds != 0 ) //als er een uur gepaseerd is wordt er 3600 bijgevoegd.
                {
                    totaalUur += 3600;
                }
            }
            int ExtraTijd = 0;
            if (totaalUur > (datenow - datefirst).TotalSeconds)//bugfix
            {
                ExtraTijd = 1;
            }
            ChartsStaVsZit.Chart = new DonutChart { Entries = staZitDagEntry, MaxValue = totaalUur-3600, LabelTextSize = 32, HoleRadius = 0.5F };
            staZitDagEntry.Add(new Entry(Convert.ToInt32(som_standing)) { Color = SkiaSharp.SKColor.Parse("#cf6b00") });
            staZitDagEntry.Add(new Entry(Convert.ToInt32(totaalUur - som_standing - (ExtraTijd*3600))) { Color = SkiaSharp.SKColor.Parse("#F9ECDF") });

            //Opvullen linkerdeel ----------------------------------
            List<TimeStandingDay> list_standing = new List<TimeStandingDay>();


            int punten = 0;
            for (int x = 0; x <= list_timestandingday.Count - 1; x++)
            {
                if (list_timestandingday[x].TimeStandingSeconds != 0)
                {
                    punten += 1;
                }
            }


           

            urenEntry.Add(new Entry(punten) { Color = SkiaSharp.SKColor.Parse("#cf6b00") });
            urenEntry.Add(new Entry(8 - punten) { Color = SkiaSharp.SKColor.Parse("#F7F7F7") });
            txtStandingHours.Text = punten.ToString(); //Deze waarde is voor de tekstbox in de ring


            ChartCirkelUren.Chart = new DonutChart { Entries = urenEntry, MaxValue = 8, HoleRadius = 0.9F };


            //Opvullen rechterdeel ---------------------------------
            for (int x = 0; x <= list_timestandingday.Count - 1; x++)
            {
                if (list_timestandingday[x].Hour < datenow.Hour)
                {
                    list_standing.Add(list_timestandingday[x]);
                }
            }

            int standing = list_standing.Count;
            if (datenow.Hour > 9)
            {
                
                var som_standing_uur = Convert.ToInt32((som_standing / 3600));
                var som_standing_min = Convert.ToInt32(som_standing / 60 % 60);
                lblUurStand.Text = (som_standing_uur).ToString() + "h " + som_standing_min.ToString() + "m";
                if (zitminuten == 60)
                {
                    standing += 1;
                    zitminuten = 0;
                }

                if ((standing - som_standing_uur - ExtraTijd) > datenow.Hour - 9)
                {
                    standing -= 1;
                }
                if (som_standing_min > zitminuten)
                {
                    zitminuten += 60;
                    ExtraTijd += 1;
                }
                lblUurZit.Text = Convert.ToString(standing - som_standing_uur - ExtraTijd) + "h " + Convert.ToString(zitminuten - som_standing_min) + "m";

            }else
            {
                lblUurStand.Text = "0h 0m";
                lblUurZit.Text = "0h 0m";
            }
                


            #endregion


        }
    }
}
