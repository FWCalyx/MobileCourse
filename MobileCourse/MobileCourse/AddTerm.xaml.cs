using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileCourse.Tables;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using Plugin.LocalNotifications;

namespace MobileCourse
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {
        public AddTerm()
        {
            InitializeComponent();
        }

        async void PopRoot()
        {
            await Navigation.PopToRootAsync();
        }


        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(termName.Text))
            {
                DisplayAlert("Invalid Name", "The term name cannot be blank.", "OK");
                return;
            }
            else if (termEnd.Date < termStart.Date)
            {
                DisplayAlert("Invalid Dates", "The end date must be later than the start date.", "OK");
                return;
            }
            else
            {
                Term term = new Term()
                {
                    Name = termName.Text,
                    Start = termStart.Date,
                    End = termEnd.Date,
                    Alert = termAlert.IsChecked,
                    DateString = $"{termStart.Date.ToShortDateString()} - {termEnd.Date.ToShortDateString()}"
                };

                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Term>();
                    if (myCon.Insert(term) == 1)
                    {
                        DisplayAlert("Term Added", "The term has been added.", "OK");
                    }
                    if (term.Alert == true)
                    {
                        CrossLocalNotifications.Current.Show($"{term.Name}", $"{term.Name} is ending.", term.ID, term.End.AddSeconds(5));
                    }
                    PopRoot();
                }
            }          
        }
    }
}