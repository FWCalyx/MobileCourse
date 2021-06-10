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
    public partial class ViewTerm : TabbedPage
    {
        Term CurrTerm = new Term();
        List<Course> courses = new List<Course>();
        public ViewTerm()
        {
            InitializeComponent();
        }
        public ViewTerm(Term chosen)
        {
            InitializeComponent();
            CurrTerm = chosen;
            TitleText.Text = $"Current Term: {CurrTerm.Name}";
            termName.Text = CurrTerm.Name;
            termStart.Date = CurrTerm.Start;
            termEnd.Date = CurrTerm.End;
            termAlert.IsChecked = CurrTerm.Alert;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
            {
                myCon.CreateTable<Course>();
                List<Course> AllCourses = myCon.Table<Course>().ToList();
                foreach (Course element in AllCourses)
                {
                    if (element.TermID == CurrTerm.ID)
                    {
                        courses.Add(element);
                    }
                }
                CoursesListView.ItemsSource = courses;
            }
        }

        async void CoursesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Course selectedCourse = e.SelectedItem as Course;
            await Navigation.PushAsync(new ViewCourse(selectedCourse));
        }

        async void NewCourse_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewCourse(CurrTerm));
        }
        async void DeleteTerm_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete Term", "All course data will be lost! Are you sure you want to delete this term?", "Yes", "No");
            if (answer == true)
            {
                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Term>();
                    myCon.CreateTable<Course>();
                    myCon.CreateTable<Assessment>();
                    myCon.CreateTable<Instructor>();
                    List<Course> course1 = myCon.Table<Course>().ToList();
                    List<Assessment> assess1 = myCon.Table<Assessment>().ToList();
                    List<Instructor> instructor1 = myCon.Table<Instructor>().ToList();
                    foreach (Course element in course1)
                    {
                        if (element.TermID == CurrTerm.ID)
                        {
                            foreach(Assessment element2 in assess1)
                            {
                                if (element2.CourseID == element.ID)
                                {
                                    myCon.Delete(element2);
                                }
                            }
                            foreach(Instructor element3 in instructor1)
                            {
                                if (element3.CourseID == element.ID)
                                {
                                    myCon.Delete(element3);
                                }
                            }
                            myCon.Delete(element);
                        }
                    }
                    if (myCon.Delete(CurrTerm) == 1)
                    {
                        await DisplayAlert("Term Deleted", "The term has been deleted.", "OK");
                        try
                        {
                            CrossLocalNotifications.Current.Cancel(CurrTerm.ID);
                        }
                        catch
                        {
                            await Navigation.PopToRootAsync();
                            return;
                        }
                        await Navigation.PopToRootAsync();

                    }

                }
            }
        }

        async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(termName.Text))
            {
                await DisplayAlert("Invalid Name", "The term name cannot be blank.", "OK");
                return;
            }
            else if (termEnd.Date < termStart.Date)
            {
                await DisplayAlert("Invalid Dates", "The end date must be later than the start date.", "OK");
                return;
            }
            else
            {
                Term Updated = new Term()
                {
                    ID = CurrTerm.ID,
                    Name = termName.Text,
                    Start = termStart.Date,
                    End = termEnd.Date,
                    Alert = termAlert.IsChecked,
                    DateString = $"{termStart.Date.ToShortDateString()} - {termEnd.Date.ToShortDateString()}"

                };

                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Term>();
                    if (myCon.Update(Updated) == 1)
                    {
                        await DisplayAlert("Term Updated", "The term has been updated.", "OK");
                        if (Updated.Alert == true)
                        {
                            CrossLocalNotifications.Current.Show($"{Updated.Name}", $"{Updated.Name} is ending.", Updated.ID, Updated.End.AddSeconds(5));
                        }
                        else
                        {
                            try
                            {
                                CrossLocalNotifications.Current.Cancel(Updated.ID);
                            }
                            catch
                            {
                                return;
                            }
                        }
                        await Navigation.PopAsync();
                    }
                
                }

            }
        }
    }
}