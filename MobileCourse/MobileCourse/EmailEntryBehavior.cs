using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace MobileCourse
{
    public class EmailEntryBehavior : Behavior<Entry>
    {
        public static string emailRegEx = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|" +
                       @"[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)" +
                       @"(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|" +
                       @"(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += EmailEntryChanged;
            base.OnAttachedTo(entry);
        }

        private void EmailEntryChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = (Entry)sender;
            if(!string.IsNullOrEmpty(entry.Text))
            {
                
                bool isMatched = Regex.IsMatch(entry.Text, emailRegEx);
                if (isMatched)
                {
                    entry.TextColor = Color.Black;
                }
                else
                {
                    entry.TextColor = Color.Red;
                }
            }
        }

        public static int EmailCheck(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {

                bool isMatched = Regex.IsMatch(email, emailRegEx);
                if (isMatched)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            base.OnDetachingFrom(entry); 
        }
    }
}
