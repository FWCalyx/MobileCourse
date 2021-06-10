using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using MobileCourse.Tables;

namespace MobileCourse
{
    public partial class MainPage : ContentPage
    {
        List<Term> terms = new List<Term>();
        List<Course> courses = new List<Course>();
        List<Assessment> assessments = new List<Assessment>();
        List<Instructor> instructors = new List<Instructor>();
        public MainPage()
        {
            InitializeComponent();
            using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
            {
                myCon.CreateTable<Term>();
                myCon.CreateTable<Course>();
                myCon.CreateTable<Assessment>();
                myCon.CreateTable<Instructor>();
                terms = myCon.Table<Term>().ToList();
                courses = myCon.Table<Course>().ToList();
                assessments = myCon.Table<Assessment>().ToList();
                instructors = myCon.Table<Instructor>().ToList();

                if (terms.Count == 0)
                {
                    DateTime startDate = new DateTime(2021, 1, 1, 0, 0, 0);
                    DateTime endDate = new DateTime(2021, 6, 30, 0, 0, 0);
                    Term term1 = new Term()
                    {
                        Name = "Spring 2021",
                        Start = startDate,
                        End = endDate,
                        Alert = false,
                        DateString = $"{startDate.Date.ToShortDateString()} - {endDate.Date.ToShortDateString()}"
                    };
                    myCon.Insert(term1);
                    terms = myCon.Table<Term>().ToList();
                    Course course1 = new Course()
                    {
                        Name = "Mobile Application Development Using C# – C971",
                        Status = "In Progress",
                        Start = startDate,
                        End = endDate,
                        Notes = "This class is fun!",
                        TermID = term1.ID,
                        Alert = true,
                        DateString = $"{startDate.Date.ToShortDateString()} - {endDate.Date.ToShortDateString()}"
                    };
                    myCon.Insert(course1);
                    Instructor instructor1 = new Instructor()
                    {
                        Name = "Rebecca Lacoste",
                        Email = "rlacos1@wgu.edu",
                        Phone = "(832)875-4722",
                        CourseID = course1.ID
                    };
                    myCon.Insert(instructor1);
                    instructors = myCon.Table<Instructor>().ToList();
                    Assessment assessPA = new Assessment()
                    {
                        Name = "C971 Performance Assessment",
                        Type = "Performance Assessment",
                        Start = startDate,
                        End = endDate,
                        Alert = false,
                        CourseID = course1.ID
                    };
                    Assessment assessOA = new Assessment()
                    {
                        Name = "C971 Objective Assessment",
                        Type = "Objective Assessment",
                        Start = startDate,
                        End = endDate,
                        Alert = false,
                        CourseID = course1.ID
                    };
                    myCon.Insert(assessPA);
                    myCon.Insert(assessOA);
                    assessments = myCon.Table<Assessment>().ToList();
                }
            }
        }

        private void NewTerm_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddTerm());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
            {
                myCon.CreateTable<Term>();
                myCon.CreateTable<Course>();
                myCon.CreateTable<Assessment>();
                myCon.CreateTable<Instructor>();
                terms = myCon.Table<Term>().ToList();
                courses = myCon.Table<Course>().ToList();
                assessments = myCon.Table<Assessment>().ToList();                
                TermListView.ItemsSource = terms;

            }
        }

        async void TermListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Term selectedTerm = e.SelectedItem as Term;
            await Navigation.PushAsync(new ViewTerm(selectedTerm));
        }

        private void CloseButton_Clicked(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
