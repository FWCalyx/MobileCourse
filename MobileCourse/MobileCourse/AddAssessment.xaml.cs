using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MobileCourse.Tables;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.LocalNotifications;

namespace MobileCourse
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewAssessment : ContentPage
    {
        int courseID;
        List<Assessment> assessments = new List<Assessment>();
        Assessment Holder = new Assessment();
        public ViewAssessment()
        {
            InitializeComponent();
        }

        public ViewAssessment(Course parent)
        {
            InitializeComponent();
            courseID = parent.ID;
            DeleteToolbarItem.IsEnabled = false;
            UpdateToolbarItem.IsEnabled = false;
        }

        public ViewAssessment(Assessment original)
        {
            InitializeComponent();
            SaveToolbarItem.IsEnabled = false;
            Holder = original;
            courseID = original.CourseID;
            assessName.Text = Holder.Name;
            assessStart.Date = Holder.Start;
            assessEnd.Date = Holder.End;
            typePicker.SelectedItem = Holder.Type;
            assessAlert.IsChecked = Holder.Alert;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
            {
                myCon.CreateTable<Assessment>();
                List<Assessment> AllAssessments = myCon.Table<Assessment>().ToList();
                foreach (Assessment element in AllAssessments)
                {
                    if (element.CourseID == courseID)
                    {
                        assessments.Add(element);
                    }
                }
            }
        }
        public int CheckAssess()
        {
            foreach (Assessment element in assessments)
            {
                if (element.Type == typePicker.SelectedItem.ToString())
                {
                    return 1;
                }
            }
            return 0;
        }
        async void PopRoot()
        {
            await Navigation.PopToRootAsync();
        }
        private void SaveAssessment_Clicked(object sender, EventArgs e)
        {
            if (CheckAssess() == 1)
            {
                DisplayAlert("Duplicate Type", "Only one of each type of assessment is permitted.", "OK");
                return;
            }
            else if (String.IsNullOrEmpty(assessName.Text))
            {
                DisplayAlert("Invalid Name", "The assessment name cannot be blank.", "OK");
                return;
            }
            else if (typePicker.SelectedIndex == -1)
            {
                DisplayAlert("Invalid Type", "The assessment type cannot be blank.", "OK");
                return;
            }
            else if (assessEnd.Date < assessStart.Date)
            {
                DisplayAlert("Invalid Dates", "The end date must be later than the start date.", "OK");
                return;
            }
            else
            {
                Assessment assessment = new Assessment()
                {
                    Name = assessName.Text,
                    Start = assessStart.Date,
                    End = assessEnd.Date,
                    Type = typePicker.SelectedItem.ToString(),
                    Alert = assessAlert.IsChecked,
                    CourseID = courseID
                };

                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Assessment>();
                    if (myCon.Insert(assessment) == 1)
                    {
                        DisplayAlert("Assessment Added", "The assessment has been added.", "OK");
                    }
                    if (assessment.Alert == true)
                    {
                        CrossLocalNotifications.Current.Show($"{assessment.Name}", $"{assessment.Type} {assessment.Name} is due.", assessment.ID, assessment.End.AddSeconds(5));
                    }
                    PopRoot();
                }

            }
        }

        private void UpdateAssessment_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(assessName.Text))
            {
                DisplayAlert("Invalid Name", "The assessment name cannot be blank.", "OK");
                return;
            }
            else if (String.IsNullOrEmpty(typePicker.SelectedItem.ToString()))
            {
                DisplayAlert("Invalid Type", "The assessment type cannot be blank.", "OK");
                return;
            }
            else if (assessEnd.Date < assessStart.Date)
            {
                DisplayAlert("Invalid Dates", "The end date must be later than the start date.", "OK");
                return;
            }
            else
            {
                Assessment assessment = new Assessment()
                {
                    ID = Holder.ID,
                    Name = assessName.Text,
                    Start = assessStart.Date,
                    End = assessEnd.Date,
                    Type = typePicker.SelectedItem.ToString(),
                    Alert = assessAlert.IsChecked,
                    CourseID = Holder.CourseID
                };

                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Assessment>();
                    if (myCon.Update(assessment) == 1)
                    {
                        DisplayAlert("Assessment Updated", "The assessment has been updated.", "OK");
                    }
                    if (assessment.Alert == true)
                    {
                        CrossLocalNotifications.Current.Show($"{assessment.Name}", $"{assessment.Type} {assessment.Name} is due.", assessment.ID, assessment.End.AddSeconds(5));
                    }
                    else
                    {
                        try
                        {
                            CrossLocalNotifications.Current.Cancel(assessment.ID);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    PopRoot();
                }
            
            }
        }

        async void DeleteAssessment_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete Assessment", "Are you sure you want to delete this assessment?", "Yes", "No");
            if (answer == true)
            {
                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Assessment>();
                    if (myCon.Delete(Holder) == 1)
                    {
                        await DisplayAlert("Assessment Deleted", "The assessment has been deleted.", "OK");
                        try
                        {
                            CrossLocalNotifications.Current.Cancel(Holder.ID);
                        }
                        catch
                        {
                            return;
                        }
                        await Navigation.PopToRootAsync();
                    }

                }
            }
        }
    }
}