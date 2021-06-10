using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using MobileCourse.Tables;


namespace MobileCourse
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewInstructor : ContentPage
    {
        int courseID;
        Instructor Holder = new Instructor();
        
        public ViewInstructor()
        {
            InitializeComponent();
        }

        public ViewInstructor(Course Parent)
        {
            InitializeComponent();
            courseID = Parent.ID;
            DeleteToolbarItem.IsEnabled = false;
            UpdateToolbarItem.IsEnabled = false;
        }

        public ViewInstructor(Instructor original)
        {
            InitializeComponent();
            SaveToolbarItem.IsEnabled = false;
            Holder = original;
            instructorName.Text = Holder.Name;
            instructorEmail.Text = Holder.Email;
            instructorPhone.Text = Holder.Phone;
        }

        async void PopRoot()
        {
            await Navigation.PopToRootAsync();
        }

        async void DeleteInstructor_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Remove Instructor", "Are you sure you want to remove this instructor?", "Yes", "No");
            if (answer == true)
            {
                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Instructor>();
                    if (myCon.Delete(Holder) == 1)
                    {
                        await DisplayAlert("Instructor Removed", "The instructor has been removed.", "OK");
                        await Navigation.PopToRootAsync();
                    }

                }
            }
        }

        private void UpdateInstructor_Clicked(object sender, EventArgs e)
        {
            if(EmailEntryBehavior.EmailCheck(instructorEmail.Text) == 0)
            {
                DisplayAlert("Invalid Email", "Email address cannot be invalid or blank.", "OK");
                return;
            }
            else if (String.IsNullOrEmpty(instructorName.Text))
            {
                DisplayAlert("Invalid Name", "The instructor name cannot be blank.", "OK");
                return;
            }
            else if (String.IsNullOrEmpty(instructorPhone.Text))
            {
                DisplayAlert("Invalid Phone", "The instructor's phone cannot be blank.", "OK");
                return;
            }
            else
            {
                Instructor instructor = new Instructor()
                {
                    ID = Holder.ID,
                    Name = instructorName.Text,
                    Email = instructorEmail.Text,
                    Phone = instructorPhone.Text,
                    CourseID = Holder.CourseID
                };

                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Instructor>();
                    if (myCon.Update(instructor) == 1)
                    {
                        DisplayAlert("Instructor Updated", "The instructor has been updated.", "OK");
                    }
                    PopRoot();
                }

            
                
            }
            

        }

        private void SaveInstructor_Clicked(object sender, EventArgs e)
        {
            if (EmailEntryBehavior.EmailCheck(instructorEmail.Text) == 0)
            {
                DisplayAlert("Invalid Email", "Email address cannot be invalid or blank.", "OK");
                return;
            }
            else if (String.IsNullOrEmpty(instructorName.Text))
            {
                DisplayAlert("Invalid Name", "The instructor name cannot be blank.", "OK");
                return;
            }
            else if (String.IsNullOrEmpty(instructorPhone.Text))
            {
                DisplayAlert("Invalid Phone", "The instructor's phone cannot be blank.", "OK");
                return;
            }
            else
            {
                Instructor instructor = new Instructor()
                {
                    Name = instructorName.Text,
                    Email = instructorEmail.Text,
                    Phone = instructorPhone.Text,
                    CourseID = courseID
                };

                using (SQLiteConnection myCon = new SQLiteConnection(App.dbPath))
                {
                    myCon.CreateTable<Instructor>();
                    if (myCon.Insert(instructor) == 1)
                    {
                        DisplayAlert("Instructor Added", "The instructor has been added.", "OK");
                    }
                    PopRoot();
                }
            }
            
        }
    }
}