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
using Xamarin.Essentials;

namespace MobileCourse
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewCourse : TabbedPage
    {
        Course Holder = new Course();
        List<Assessment> assessments = new List<Assessment>();
        List<Instructor> instructors = new List<Instructor>();

        public ViewCourse()
        {
            InitializeComponent();
        }
        public ViewCourse(Term currTerm)
        {
            InitializeComponent();
            Holder.TermID = currTerm.ID;
            DeleteButton.IsEnabled = false;
            UpdateButton.IsEnabled = false;

        }
        public ViewCourse(Course chosen)
        {
            InitializeComponent();
            Holder = chosen;
            SaveButton.IsEnabled = false;
            courseName.Text = Holder.Name;
            statusPicker.SelectedItem = Holder.Status;
            courseStart.Date = Holder.Start;
            courseEnd.Date = Holder.End;
            courseNotes.Text = Holder.Notes;
            courseAlert.IsChecked = Holder.Alert;            
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
                    if (element.CourseID == Holder.ID)
                    {
                        assessments.Add(element);
                    }
                }
                if (assessments.Count >= 2)
                {
                    addAssessButton.IsEnabled = false;
                }
                AssessmentsListView.ItemsSource = assessments;
                myCon.CreateTable<Instructor>();
                List<Instructor> AllInstructors = myCon.Table<Instructor>().ToList();
                foreach (Instructor element in AllInstructors)
                {
                    if(element.CourseID == Holder.ID)
                    {
                        instructors.Add(element);
                    }
                }
                InstructorsListView.ItemsSource = instructors;
            }
        }
        async void PopRoot()
        {
            await Navigation.PopToRootAsync();
        }



        async void DeleteCourse_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete Course", "Are you sure you want to delete this course?", "Yes", "No");
            if (answer == true)
            {
                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {

                    myCon.CreateTable<Course>();
                    myCon.CreateTable<Assessment>();
                    myCon.CreateTable<Instructor>();
                    List<Assessment> assess1 = myCon.Table<Assessment>().ToList();
                    foreach(Assessment element in assess1)
                    {
                        if (element.CourseID == Holder.ID)
                        {
                            myCon.Delete(element);
                        }
                    }
                    List<Instructor> instructor1 = myCon.Table<Instructor>().ToList();
                    foreach(Instructor element in instructor1)
                    {
                        if (element.CourseID == Holder.ID)
                        {
                            myCon.Delete(element);
                        }
                    }
                    if (myCon.Delete(Holder) == 1)
                    {
                        await DisplayAlert("Course Deleted", "The course has been deleted.", "OK");
                        try
                        {
                            CrossLocalNotifications.Current.Cancel(Holder.ID);
                        }
                        catch
                        {
                            PopRoot();
                            return;
                        }
                        PopRoot();
                    }

                }
            }
        }

        private void AddAssessment_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ViewAssessment(Holder));
        }

        private void AddInstructor_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ViewInstructor(Holder));
        }


        private void SaveCourse_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(courseName.Text))
            {
                DisplayAlert("Invalid Name", "The course name cannot be blank.", "OK");
                return;
            }
            else if (statusPicker.SelectedIndex == -1)
            {
                DisplayAlert("Invalid Status", "The course status name cannot be blank.", "OK");
                return;
            }
            else if (courseEnd.Date < courseStart.Date)
            {
                DisplayAlert("Invalid Dates", "The end date must be later than the start date.", "OK");
                return;
            }
            else
            {
                Course course1 = new Course()
                {
                    Name = courseName.Text,
                    Status = statusPicker.SelectedItem.ToString(),
                    Start = courseStart.Date,
                    End = courseEnd.Date,
                    Notes = courseNotes.Text,
                    TermID = Holder.TermID,
                    Alert = courseAlert.IsChecked,
                    DateString = $"{courseStart.Date.ToShortDateString()} - {courseEnd.Date.ToShortDateString()}"
                };
                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Course>();
                    if (myCon.Insert(course1) == 1)
                    {
                        DisplayAlert("Course Added", "The course has been added.", "OK");
                    }
                    if (course1.Alert == true)
                    {
                        CrossLocalNotifications.Current.Show($"{course1.Name}", $"{course1.Name} is ending.", course1.ID, course1.End.AddSeconds(5));
                    }
                    PopRoot();
                }
            
            }
        }

        private void UpdateCourse_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(courseName.Text))
            {
                DisplayAlert("Invalid Name", "The course name cannot be blank.", "OK");
                return;
            }
            else if (String.IsNullOrEmpty(statusPicker.SelectedItem.ToString()))
            {
                DisplayAlert("Invalid Status", "The course status name cannot be blank.", "OK");
                return;
            }
            else if (courseEnd.Date < courseStart.Date)
            {
                DisplayAlert("Invalid Dates", "The end date must be later than the start date.", "OK");
                return;
            }
            else
            {
                Course course1 = new Course()
                {
                    ID = Holder.ID,
                    Name = courseName.Text,
                    Status = statusPicker.SelectedItem.ToString(),
                    Start = courseStart.Date,
                    End = courseEnd.Date,
                    Notes = courseNotes.Text,
                    TermID = Holder.TermID,
                    Alert = courseAlert.IsChecked,
                    DateString = $"{courseStart.Date.ToShortDateString()} - {courseEnd.Date.ToShortDateString()}"
                };
                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Course>();
                    if (myCon.Update(course1) == 1)
                    {
                        DisplayAlert("Course Updated", "The course has been updated.", "OK");
                    }
                    if (course1.Alert == true)
                    {
                        CrossLocalNotifications.Current.Show($"{course1.Name}", $"{course1.Name} is ending.", course1.ID, course1.End.AddSeconds(5));
                    }
                    else
                    {
                        try
                        {
                            CrossLocalNotifications.Current.Cancel(course1.ID);
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

        async void AssessmentsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Assessment selectedAssessment = e.SelectedItem as Assessment;
            await Navigation.PushAsync(new ViewAssessment(selectedAssessment));
        }

        async void InstructorsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Instructor selectedInstructor = e.SelectedItem as Instructor;
            await Navigation.PushAsync(new ViewInstructor(selectedInstructor));
        }

        async void ShareButton_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = courseNotes.Text,
                Title = $"Course Notes for {courseName.Text}"
            });
        }
    }
}